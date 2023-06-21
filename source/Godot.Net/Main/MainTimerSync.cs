namespace Godot.Net.Main;

using Godot.Net.Core.Config;

public partial class MainTimerSync
{
    private const int CONTROL_STEPS = 12;
    private static bool firstPrint = true;

    private readonly int[] accumulatedPhysicsSteps = new int[12];
    private readonly int[] typicalPhysicsSteps     = new int[12];

    private long   lastCpuTicksUsec;
    private double timeAccum;
    private double timeAccumulated;
    private double timeDeficit;

    public long CpuTicksUsec { get; set; }
    public int  FixedFps     { get; set; }

    private MainFrameTime AdvanceCore(double physicsStep, int physicsTicksPerSecond, double processStep)
    {
        this.timeAccumulated += processStep;

        var ret = new MainFrameTime()
        {
            ProcessStep = processStep,
            PhysicsSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond),
        };

        var minTypicalSteps = this.typicalPhysicsSteps[0];
        var maxTypicalSteps = minTypicalSteps + 1;

        var updateTypical = false;

        for (var i = 0; i < CONTROL_STEPS - 1; i++)
        {
            var stepsLeftToMatchTypical = this.typicalPhysicsSteps[i + 1] - this.accumulatedPhysicsSteps[i];

            if (stepsLeftToMatchTypical > maxTypicalSteps || stepsLeftToMatchTypical + 1 < minTypicalSteps)
            {
                updateTypical = true;

                break;
            }

            if (stepsLeftToMatchTypical > minTypicalSteps)
            {
                minTypicalSteps = stepsLeftToMatchTypical;
            }
            if (stepsLeftToMatchTypical + 1 < maxTypicalSteps)
            {
                maxTypicalSteps = stepsLeftToMatchTypical + 1;
            }
        }

        #if DEBUG
        if (maxTypicalSteps < 0)
        {
            WARN_PRINT_ONCE($"`{nameof(maxTypicalSteps)}` is negative. This could hint at an engine bug or system timer misconfiguration.", ref firstPrint);
        }
        #endif

        if (ret.PhysicsSteps < minTypicalSteps)
        {
            var maxPossibleSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond + Engine.Singleton.PhysicsJitterFix);

            if (maxPossibleSteps < minTypicalSteps)
            {
                ret.PhysicsSteps = maxPossibleSteps;
                updateTypical = true;
            }
            else
            {
                ret.PhysicsSteps = minTypicalSteps;
            }
        }
        else if (ret.PhysicsSteps > maxTypicalSteps)
        {
            var minPossibleSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond - Engine.Singleton.PhysicsJitterFix);

            if (minPossibleSteps > maxTypicalSteps)
            {
                ret.PhysicsSteps = minPossibleSteps;
                updateTypical = true;
            }
            else
            {
                ret.PhysicsSteps = maxTypicalSteps;
            }
        }

        if (ret.PhysicsSteps < 0)
        {
            ret.PhysicsSteps = 0;
        }

        this.timeAccumulated -= ret.PhysicsSteps * physicsStep;

        // keep track of accumulated step counts
        for (var i = CONTROL_STEPS - 2; i >= 0; --i)
        {
            this.accumulatedPhysicsSteps[i + 1] = this.accumulatedPhysicsSteps[i] + ret.PhysicsSteps;
        }
        this.accumulatedPhysicsSteps[0] = ret.PhysicsSteps;

        if (updateTypical)
        {
            for (var i = CONTROL_STEPS - 1; i >= 0; --i)
            {
                if (this.typicalPhysicsSteps[i] > this.accumulatedPhysicsSteps[i])
                {
                    this.typicalPhysicsSteps[i] = this.accumulatedPhysicsSteps[i];
                }
                else if (this.typicalPhysicsSteps[i] < this.accumulatedPhysicsSteps[i] - 1)
                {
                    this.typicalPhysicsSteps[i] = this.accumulatedPhysicsSteps[i] - 1;
                }
            }
        }

        return ret;
    }

    private int GetAveragePhysicsSteps(out double min, out double max)
    {
        min = this.typicalPhysicsSteps[0];
        max = min + 1;

        for (var i = 1; i < CONTROL_STEPS; ++i)
        {
            var typicalLower = this.typicalPhysicsSteps[i];
            var currentMin   = typicalLower / (i + 1);

            if (currentMin > max)
            {
                return i; // bail out if further restrictions would void the interval
            }
            else if (currentMin > min)
            {
                min = currentMin;
            }

            var currentMax = (typicalLower + 1) / (i + 1);

            if (currentMax < min)
            {
                return i;
            }
            else if (currentMax < max)
            {
                max = currentMax;
            }
        }

        return CONTROL_STEPS;
    }

    private double GetCpuProcessStep()
    {
        var cpuTicksElapsed = this.CpuTicksUsec - this.lastCpuTicksUsec;

        this.lastCpuTicksUsec = this.CpuTicksUsec;

        return cpuTicksElapsed / 1000000.0;
    }

    public MainFrameTime Advance(double physicsStep, int physicsTicksPerSecond)
    {
        var cpuProcessStep = this.GetCpuProcessStep();

        return this.AdvanceChecked(physicsStep, physicsTicksPerSecond, cpuProcessStep);
    }

    public MainFrameTime AdvanceChecked(double physicsStep, int physicsTicksPerSecond, double processStep)
    {
        if (this.FixedFps != -1)
        {
            processStep = 1.0 / this.FixedFps;
        }

        var minOutputStep = processStep / 8;
        minOutputStep = Math.Max(minOutputStep, 1E-6);

        // compensate for last deficit
        processStep += this.timeDeficit;

        var ret = this.AdvanceCore(physicsStep, physicsTicksPerSecond, processStep);

        // we will do some clamping on ret.ProcessStep and need to sync those changes to timeAccum,
        // that's easiest if we just remember their fixed difference now
        var processMinusAccum = ret.ProcessStep - this.timeAccum;

        // first, least important clamping: keep ret.ProcessStep consistent with typical_physics_steps.
        // this smoothes out the process steps and culls small but quick variations.
        {
            var consistentSteps = this.GetAveragePhysicsSteps(out var minAveragePhysicsSteps, out var maxAveragePhysicsSteps);
            if (consistentSteps > 3)
            {
                ret.ClampProcessStep(minAveragePhysicsSteps * physicsStep, maxAveragePhysicsSteps * physicsStep);
            }
        }

        // second clamping: keep abs(timeDeficit) < jitter_fix * frame_slise
        var maxClockDeviation = Engine.Singleton.PhysicsJitterFix * physicsStep;
        ret.ClampProcessStep(processStep - maxClockDeviation, processStep + maxClockDeviation);

        // last clamping: make sure timeAccum is between 0 and physicsStep for consistency between physics and process
        ret.ClampProcessStep(processMinusAccum, processMinusAccum + physicsStep);

        // all the operations above may have turned ret.processStep negative or zero, keep a minimal value
        if (ret.ProcessStep < minOutputStep)
        {
            ret.ProcessStep = minOutputStep;
        }

        // restore timeAccum
        this.timeAccum = ret.ProcessStep - processMinusAccum;

        // forcing ret.ProcessStep to be positive may trigger a violation of the
        // promise that timeAccum is between 0 and physicsStep
        #if DEBUG
        if (this.timeAccum < -1E-7)
        {
            WARN_PRINT_ONCE("Intermediate value of `timeAccum` is negative. This could hint at an engine bug or system timer misconfiguration.", ref firstPrint);
        }
        #endif

        if (this.timeAccum > physicsStep)
        {
            var extraPhysicsSteps = (int)(this.timeAccum * physicsTicksPerSecond);
            this.timeAccum -= extraPhysicsSteps * physicsStep;
            ret.PhysicsSteps += extraPhysicsSteps;
        }

        #if DEBUG
        if (this.timeAccum < -1E-7)
        {
            WARN_PRINT_ONCE("Final value of `timeAccum` is negative. It should always be between 0 and `physicsStep`. This hints at an engine bug.", ref firstPrint);
        }
        if (this.timeAccum > physicsStep + 1E-7)
        {
            WARN_PRINT_ONCE("Final value of `timeAccum` is larger than `physicsStep`. It should always be between 0 and `physicsStep`. This hints at an engine bug.", ref firstPrint);
        }
        #endif

        // track deficit
        this.timeDeficit = processStep - ret.ProcessStep;

        // physicsStep is 1.0 / iterations_per_sec
        // i.e. the time in seconds taken by a physics tick
        ret.InterpolationFraction = this.timeAccum / physicsStep;

        return ret;
    }

    public void Init(long cpuTicksUsec) =>
        this.CpuTicksUsec = this.lastCpuTicksUsec = cpuTicksUsec;
}
