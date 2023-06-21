namespace Godot.Net.Core.Config;

using System.Runtime.CompilerServices;

#pragma warning disable IDE0052, CS0414 // TODO Remove

public partial class ProjectSettings
{
    public static class Macros
    {
        private static T P_GLOBAL_DEF<T>(
            string name,
            T      @default,
            bool   restartIfChanged  = default,
            bool   ignoreValueInDocs = default,
            bool   basic             = default,
            bool   @internal         = default
        )
        {
            if (!Singleton.HasSetting(name))
            {
                Singleton.Set(name, @default);
            }

            var ret = GLOBAL_GET<T>(name)!;

            if (!ERR_FAIL_COND_MSG(!Singleton.props.TryGetValue(name, out var container), $"Request for nonexistent project setting: {name}."))
            {
                container!.Basic             = basic;
                container!.RestartIfChanged  = restartIfChanged;
                container!.IgnoreValueInDocs = ignoreValueInDocs;
                container!.Internal          = @internal;

                SetInitialValue(container, @default);
                Singleton.SetBuiltinOrder(container);
            }

            return ret;
        }

        private static T P_GLOBAL_DEF<T>(
            PropertyInfo info,
            T            @default,
            bool         restartIfChanged  = default,
            bool         ignoreValueInDocs = default,
            bool         basic             = default,
            bool         @internal         = default
        )
        {
            var ret = P_GLOBAL_DEF(info.Name, @default, restartIfChanged, ignoreValueInDocs, basic, @internal);

            Singleton.SetCustomPropertyInfo(info);

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF<T>(string key, T value) => P_GLOBAL_DEF(key, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF<T>(PropertyInfo propertyInfo, T value) => P_GLOBAL_DEF(propertyInfo, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T? GLOBAL_GET<T>(string key) => Singleton.GetSettingWithOverride<T>(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_BASIC<T>(string key, T value) => P_GLOBAL_DEF(key, value, false, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_BASIC<T>(PropertyInfo propertyInfo, T value) => P_GLOBAL_DEF(propertyInfo, value, false, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_INTERNAL<T>(string key, T value) => P_GLOBAL_DEF(key, value, false, false, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST<T>(string key, T value) => P_GLOBAL_DEF(key, value, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST<T>(PropertyInfo propertyInfo, T value) => P_GLOBAL_DEF(propertyInfo, value, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST_BASIC<T>(string key, T value) => P_GLOBAL_DEF(key, value, true, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST_BASIC<T>(PropertyInfo propertyInfo, T value) => P_GLOBAL_DEF(propertyInfo, value, true, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST_NOVAL<T>(string key, T value) => P_GLOBAL_DEF(key, value, true, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T GLOBAL_DEF_RST_NOVAL<T>(PropertyInfo propertyInfo, T value) => P_GLOBAL_DEF(propertyInfo, value, true, true);
    }
}

