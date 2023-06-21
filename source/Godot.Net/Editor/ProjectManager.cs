#define MODULE_SVG_ENABLED

namespace Godot.Net.Editor;

using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Scene.GUI;
using Godot.Net.Scene.Main;
using Godot.Net.Servers;

using static EditorSettings.Macros;

public class ProjectManager : Control
{
    private readonly TabContainer  tabs;
    private readonly HBoxContainer localProjectsHb;
    private readonly LineEdit      searchBox;
    private readonly Label         loadingLabel;

    public static ProjectManager Singleton { get; private set; } = null!;

    public bool ProcessShortcutInput
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ProjectManager()
    {
        Singleton = this;

        // load settings
        if (!EditorSettings.IsInitialized)
        {
            EditorSettings.Create();
        }

        #region TODO
        //     // Turn off some servers we aren't going to be using in the Project Manager.
        //     NavigationServer3D::get_singleton().set_active(false);
        //     PhysicsServer3D::get_singleton().set_active(false);
        //     PhysicsServer2D::get_singleton().set_active(false);
        #endregion TODO

        EditorSettings.Singleton.OptimizeSave = false; //just write settings as they came

        {
            var displayScale = EDITOR_GET<int>("interface/editor/display_scale");

            EditorScale.Value = displayScale switch
            {
                0 => EditorSettings.AutoDisplayScale,// Try applying a suitable display scale automatically.
                1 => 0.75f,
                2 => 1.0f,
                3 => 1.25f,
                4 => 1.5f,
                5 => 1.75f,
                6 => 2.0f,
                _ => EDITOR_GET<float>("interface/editor/custom_display_scale"),
            };

            #region TODO
            // EditorFileDialog.GetIconFunc      = ProjectManager.FileDialogGetIcon;
            // EditorFileDialog.GetThumbnailFunc = ProjectManager.FileDialogGetThumbnail;
            #endregion TODO
        }

        // TRANSLATORS: This refers to the application where users manage their Godot projects.
        DisplayServer.Singleton.WindowSetTitle($"{GodotVersion.VERSION_NAME} - {TTR("Project Manager", "Application")}");

        #region TODO
        // EditorFileDialog::set_default_show_hidden_files(EDITOR_GET("filesystem/file_dialog/show_hidden_files"));

        // int swap_cancel_ok = EDITOR_GET("interface/editor/accept_dialog_cancel_ok_buttons");
        // if (swap_cancel_ok != 0) { // 0 is auto, set in register_scene based on DisplayServer.
        //     // Swap on means OK first.
        //     AcceptDialog::set_swap_cancel_ok(swap_cancel_ok == 2);
        // }
        #endregion TODO

        var theme = EditorThemes.CreateCustomTheme();

        this.Theme = theme;

        DisplayServer.SetEarlyWindowClearColorOverride(true, theme.GetColor("background", "Editor"));

        this.SetAnchorsAndOffsetsPreset(LayoutPreset.PRESET_FULL_RECT);

        var panel = new Panel { PostInitialize = true };
        this.AddChild(panel);
        panel.SetAnchorsAndOffsetsPreset(LayoutPreset.PRESET_FULL_RECT);
        panel.AddThemeStyleOverride("panel", this.GetThemeStylebox("Background", "EditorStyles"));

        var vb = new VBoxContainer { PostInitialize = true };
        panel.AddChild(vb);
        vb.SetAnchorsAndOffsetsPreset(LayoutPreset.PRESET_FULL_RECT, LayoutPresetMode.PRESET_MODE_MINSIZE, 8 * (int)EditorScale.Value);

        var centerBox = new Control
        {
            PostInitialize = true,
            VSizeFlags     = SizeFlags.SIZE_EXPAND_FILL,
        };

        vb.AddChild(centerBox);

        this.tabs = new TabContainer { PostInitialize = true };
        centerBox.AddChild(this.tabs);
        this.tabs.SetAnchorsAndOffsetsPreset(LayoutPreset.PRESET_FULL_RECT);
        this.tabs.TabChanged += this.OnTabChanged;

        this.localProjectsHb = new HBoxContainer
        {
            PostInitialize = true,
            Name           = TTR("Local Projects"),
        };

        this.tabs.AddChild(this.localProjectsHb);

        {
            // Projects + search bar
            var searchTreeVb = new VBoxContainer { PostInitialize = true };
            this.localProjectsHb.AddChild(searchTreeVb);
            searchTreeVb.HSizeFlags = SizeFlags.SIZE_EXPAND_FILL;

            var hb = new HBoxContainer
            {
                PostInitialize = true,
                HSizeFlags     = SizeFlags.SIZE_EXPAND_FILL,
            };

            searchTreeVb.AddChild(hb);

            this.searchBox = new LineEdit
            {
                PostInitialize = true,
                Placeholder    = TTR("Filter Projects"),
                TooltipText    = TTR("This field filters projects by name and last path component.\nTo filter projects by name and full path, the query must contain at least one `/` character."),
            };

            this.searchBox.TextChanged += this.OnSearchTermChanged;
            this.searchBox.HSizeFlags = SizeFlags.SIZE_EXPAND_FILL;

            hb.AddChild(this.searchBox);

            this.loadingLabel = new Label(TTR("Loading, please wait...")) { PostInitialize = true };
            this.loadingLabel.AddThemeFontOverride("font", this.GetThemeFont("bold", "EditorFonts"));
            this.loadingLabel.HSizeFlags = SizeFlags.SIZE_EXPAND_FILL;
            hb.AddChild(this.loadingLabel);
            // The loading label is shown later.
            this.loadingLabel.Hide();

            var sortLabel = new Label
            {
                Text           = TTR("Sort:"),
                PostInitialize = true
            };

            hb.AddChild(sortLabel);

        #region TODO
        //         filter_option = new OptionButton();
        //         filter_option.set_clip_text(true);
        //         filter_option.set_h_size_flags(Control::SIZE_EXPAND_FILL);
        //         filter_option.connect("item_selected", callable_mp(this, &ProjectManager::_on_order_option_changed));
        //         hb.add_child(filter_option);

        //         Vector<String> sort_filter_titles;
        //         sort_filter_titles.push_back(TTR("Last Edited"));
        //         sort_filter_titles.push_back(TTR("Name"));
        //         sort_filter_titles.push_back(TTR("Path"));

        //         for (int i = 0; i < sort_filter_titles.size(); i++) {
        //             filter_option.add_item(sort_filter_titles[i]);
        //         }

        //         PanelContainer *pc = new PanelContainer();
        //         pc.add_theme_style_override("panel", get_theme_stylebox(("panel"), ("Tree")));
        //         pc.set_v_size_flags(Control::SIZE_EXPAND_FILL);
        //         searchTreeVb.add_child(pc);

        //         _project_list = new ProjectList();
        //         _project_list.connect(ProjectList::SIGNAL_SELECTION_CHANGED, callable_mp(this, &ProjectManager::_update_project_buttons));
        //         _project_list.connect(ProjectList::SIGNAL_PROJECT_ASK_OPEN, callable_mp(this, &ProjectManager::_open_selected_projects_ask));
        //         _project_list.set_horizontal_scroll_mode(ScrollContainer::SCROLL_MODE_DISABLED);
        //         pc.add_child(_project_list);
        #endregion TODO
        }

        #region TODO
        //     {
        //         // Project tab side bar
        //         VBoxContainer *tree_vb = new VBoxContainer();
        //         tree_vb.set_custom_minimum_size(Size2(120, 120));
        //         localProjectsHb.add_child(tree_vb);

        //         var btn_h_separation = int(6 * EditorScale.Value);

        //         create_btn = new Button();
        //         create_btn.set_text(TTR("New Project"));
        //         create_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         create_btn.set_shortcut(ED_SHORTCUT("project_manager/new_project", TTR("New Project"), KeyModifierMask::CMD_OR_CTRL | Key::N));
        //         create_btn.connect("pressed", callable_mp(this, &ProjectManager::_new_project));
        //         tree_vb.add_child(create_btn);

        //         import_btn = new Button();
        //         import_btn.set_text(TTR("Import"));
        //         import_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         import_btn.set_shortcut(ED_SHORTCUT("project_manager/import_project", TTR("Import Project"), KeyModifierMask::CMD_OR_CTRL | Key::I));
        //         import_btn.connect("pressed", callable_mp(this, &ProjectManager::_import_project));
        //         tree_vb.add_child(import_btn);

        //         scan_btn = new Button();
        //         scan_btn.set_text(TTR("Scan"));
        //         scan_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         scan_btn.set_shortcut(ED_SHORTCUT("project_manager/scan_projects", TTR("Scan Projects"), KeyModifierMask::CMD_OR_CTRL | Key::S));
        //         scan_btn.connect("pressed", callable_mp(this, &ProjectManager::_scan_projects));
        //         tree_vb.add_child(scan_btn);

        //         tree_vb.add_child(memnew(HSeparator));

        //         open_btn = new Button();
        //         open_btn.set_text(TTR("Edit"));
        //         open_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         open_btn.set_shortcut(ED_SHORTCUT("project_manager/edit_project", TTR("Edit Project"), KeyModifierMask::CMD_OR_CTRL | Key::E));
        //         open_btn.connect("pressed", callable_mp(this, &ProjectManager::_open_selected_projects_ask));
        //         tree_vb.add_child(open_btn);

        //         run_btn = new Button();
        //         run_btn.set_text(TTR("Run"));
        //         run_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         run_btn.set_shortcut(ED_SHORTCUT("project_manager/run_project", TTR("Run Project"), KeyModifierMask::CMD_OR_CTRL | Key::R));
        //         run_btn.connect("pressed", callable_mp(this, &ProjectManager::_run_project));
        //         tree_vb.add_child(run_btn);

        //         rename_btn = new Button();
        //         rename_btn.set_text(TTR("Rename"));
        //         rename_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         // The F2 shortcut isn't overridden with Enter on macOS as Enter is already used to edit a project.
        //         rename_btn.set_shortcut(ED_SHORTCUT("project_manager/rename_project", TTR("Rename Project"), Key::F2));
        //         rename_btn.connect("pressed", callable_mp(this, &ProjectManager::_rename_project));
        //         tree_vb.add_child(rename_btn);

        //         erase_btn = new Button();
        //         erase_btn.set_text(TTR("Remove"));
        //         erase_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         erase_btn.set_shortcut(ED_SHORTCUT("project_manager/remove_project", TTR("Remove Project"), Key::KEY_DELETE));
        //         erase_btn.connect("pressed", callable_mp(this, &ProjectManager::_erase_project));
        //         tree_vb.add_child(erase_btn);

        //         erase_missing_btn = new Button();
        //         erase_missing_btn.set_text(TTR("Remove Missing"));
        //         erase_missing_btn.add_theme_constant_override("h_separation", btn_h_separation);
        //         erase_missing_btn.connect("pressed", callable_mp(this, &ProjectManager::_erase_missing_projects));
        //         tree_vb.add_child(erase_missing_btn);

        //         tree_vb.add_spacer();

        //         about_btn = new Button();
        //         about_btn.set_text(TTR("About"));
        //         about_btn.connect("pressed", callable_mp(this, &ProjectManager::_show_about));
        //         tree_vb.add_child(about_btn);
        //     }

        //     {
        //         // Version info and language options
        //         settings_hb = new HBoxContainer();
        //         settings_hb.set_alignment(BoxContainer::ALIGNMENT_END);
        //         settings_hb.set_h_grow_direction(Control::GROW_DIRECTION_BEGIN);
        //         settings_hb.set_anchors_and_offsets_preset(Control::PRESET_TOP_RIGHT);

        //         // A VBoxContainer that contains a dummy Control node to adjust the LinkButton's vertical position.
        //         VBoxContainer *spacer_vb = new VBoxContainer();
        //         settings_hb.add_child(spacer_vb);

        //         Control *v_spacer = new Control();
        //         spacer_vb.add_child(v_spacer);

        //         version_btn = new LinkButton();
        //         String hash = String(VERSION_HASH);
        //         if (hash.length() != 0) {
        //             hash = " " + vformat("[%s]", hash.left(9));
        //         }
        //         version_btn.set_text("v" VERSION_FULL_BUILD + hash);
        //         // Fade the version label to be less prominent, but still readable.
        //         version_btn.set_self_modulate(new Color(1, 1, 1, 0.6));
        //         version_btn.set_underline_mode(LinkButton::UNDERLINE_MODE_ON_HOVER);
        //         version_btn.set_tooltip_text(TTR("Click to copy."));
        //         version_btn.connect("pressed", callable_mp(this, &ProjectManager::_version_button_pressed));
        //         spacer_vb.add_child(version_btn);

        //         // Add a small horizontal spacer between the version and language buttons
        //         // to distinguish them.
        //         Control *h_spacer = new Control();
        //         settings_hb.add_child(h_spacer);

        //         language_btn = new OptionButton();
        //         language_btn.set_icon(get_theme_icon(("Environment"), ("EditorIcons")));
        //         language_btn.set_focus_mode(Control::FOCUS_NONE);
        //         language_btn.set_fit_to_longest_item(false);
        //         language_btn.set_flat(true);
        //         language_btn.connect("item_selected", callable_mp(this, &ProjectManager::_language_selected));
        // #ifdef ANDROID_ENABLED
        //         // The language selection dropdown doesn't work on Android (as the setting isn't saved), see GH-60353.
        //         // Also, the dropdown it spawns is very tall and can't be scrolled without a hardware mouse.
        //         // Hiding the language selection dropdown also leaves more space for the version label to display.
        //         language_btn.hide();
        // #endif

        //         Vector<String> editor_languages;
        //         List<PropertyInfo> editor_settings_properties;
        //         EditorSettings.Singleton.get_property_list(&editor_settings_properties);
        //         for (const PropertyInfo &pi : editor_settings_properties) {
        //             if (pi.name == "interface/editor/editor_language") {
        //                 editor_languages = pi.hint_string.split(",");
        //                 break;
        //             }
        //         }

        //         String current_lang = EDITOR_GET("interface/editor/editor_language");
        //         language_btn.set_text(current_lang);

        //         for (int i = 0; i < editor_languages.size(); i++) {
        //             String lang = editor_languages[i];
        //             String lang_name = TranslationServer::get_singleton().get_locale_name(lang);
        //             language_btn.add_item(vformat("[%s] %s", lang, lang_name), i);
        //             language_btn.set_item_metadata(i, lang);
        //             if (current_lang == lang) {
        //                 language_btn.select(i);
        //             }
        //         }

        //         settings_hb.add_child(language_btn);
        //         center_box.add_child(settings_hb);
        //     }

        //     if (AssetLibraryEditorPlugin::is_available()) {
        //         asset_library = memnew(EditorAssetLibrary(true));
        //         asset_library.set_name(TTR("Asset Library Projects"));
        //         tabs.add_child(asset_library);
        //         asset_library.connect("install_asset", callable_mp(this, &ProjectManager::_install_project));
        //     } else {
        //         print_verbose("Asset Library not available (due to using Web editor, or SSL support disabled).");
        //     }

        //     {
        //         // Dialogs
        //         language_restart_ask = new ConfirmationDialog();
        //         language_restart_ask.set_ok_button_text(TTR("Restart Now"));
        //         language_restart_ask.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_restart_confirm));
        //         language_restart_ask.set_cancel_button_text(TTR("Continue"));
        //         add_child(language_restart_ask);

        //         scan_dir = new EditorFileDialog();
        //         scan_dir.set_previews_enabled(false);
        //         scan_dir.set_access(EditorFileDialog::ACCESS_FILESYSTEM);
        //         scan_dir.set_file_mode(EditorFileDialog::FILE_MODE_OPEN_DIR);
        //         scan_dir.set_title(TTR("Select a Folder to Scan")); // must be after mode or it's overridden
        //         scan_dir.set_current_dir(EDITOR_GET("filesystem/directories/default_project_path"));
        //         add_child(scan_dir);
        //         scan_dir.connect("dir_selected", callable_mp(this, &ProjectManager::_scan_begin));

        //         erase_missing_ask = new ConfirmationDialog();
        //         erase_missing_ask.set_ok_button_text(TTR("Remove All"));
        //         erase_missing_ask.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_erase_missing_projects_confirm));
        //         add_child(erase_missing_ask);

        //         erase_ask = new ConfirmationDialog();
        //         erase_ask.set_ok_button_text(TTR("Remove"));
        //         erase_ask.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_erase_project_confirm));
        //         add_child(erase_ask);

        //         VBoxContainer *erase_ask_vb = new VBoxContainer();
        //         erase_ask.add_child(erase_ask_vb);

        //         erase_ask_label = new Label();
        //         erase_ask_vb.add_child(erase_ask_label);

        //         delete_project_contents = new CheckBox();
        //         delete_project_contents.set_text(TTR("Also delete project contents (no undo!)"));
        //         erase_ask_vb.add_child(delete_project_contents);

        //         multi_open_ask = new ConfirmationDialog();
        //         multi_open_ask.set_ok_button_text(TTR("Edit"));
        //         multi_open_ask.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_open_selected_projects));
        //         add_child(multi_open_ask);

        //         multi_run_ask = new ConfirmationDialog();
        //         multi_run_ask.set_ok_button_text(TTR("Run"));
        //         multi_run_ask.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_run_project_confirm));
        //         add_child(multi_run_ask);

        //         multi_scan_ask = new ConfirmationDialog();
        //         multi_scan_ask.set_ok_button_text(TTR("Scan"));
        //         add_child(multi_scan_ask);

        //         ask_update_settings = new ConfirmationDialog();
        //         ask_update_settings.set_autowrap(true);
        //         ask_update_settings.get_ok_button().connect("pressed", callable_mp(this, &ProjectManager::_confirm_update_settings));
        //         full_convert_button = ask_update_settings.add_button("Convert Full Project", !GLOBAL_GET("gui/common/swap_cancel_ok"));
        //         full_convert_button.connect("pressed", callable_mp(this, &ProjectManager::_full_convert_button_pressed));
        //         add_child(ask_update_settings);

        //         ask_full_convert_dialog = new ConfirmationDialog();
        //         ask_full_convert_dialog.set_autowrap(true);
        //         ask_full_convert_dialog.set_text(TTR("This option will perform full project conversion, updating scenes, resources and scripts from Godot 3.x to work in Godot 4.0.\n\nNote that this is a best-effort conversion, i.e. it makes upgrading the project easier, but it will not open out-of-the-box and will still require manual adjustments.\n\nIMPORTANT: Make sure to backup your project before converting, as this operation makes it impossible to open it in older versions of Godot."));
        //         ask_full_convert_dialog.connect("confirmed", callable_mp(this, &ProjectManager::_perform_full_project_conversion));
        //         add_child(ask_full_convert_dialog);

        //         npdialog = new ProjectDialog();
        //         npdialog.connect("projects_updated", callable_mp(this, &ProjectManager::_on_projects_updated));
        //         npdialog.connect("project_created", callable_mp(this, &ProjectManager::_on_project_created));
        //         add_child(npdialog);

        //         run_error_diag = new AcceptDialog();
        //         run_error_diag.set_title(TTR("Can't run project"));
        //         add_child(run_error_diag);

        //         dialog_error = new AcceptDialog();
        //         add_child(dialog_error);

        //         if (asset_library) {
        //             open_templates = new ConfirmationDialog();
        //             open_templates.set_text(TTR("You currently don't have any projects.\nWould you like to explore official example projects in the Asset Library?"));
        //             open_templates.set_ok_button_text(TTR("Open Asset Library"));
        //             open_templates.connect("confirmed", callable_mp(this, &ProjectManager::_open_asset_library));
        //             add_child(open_templates);
        //         }

        //         about = new EditorAbout();
        //         add_child(about);

        //         _build_icon_type_cache(get_theme());
        //     }

        //     _project_list.migrate_config();
        //     _load_recent_projects();

        //     Ref<DirAccess> dir_access = DirAccess::create(DirAccess::AccessType::ACCESS_FILESYSTEM);

        //     String default_project_path = EDITOR_GET("filesystem/directories/default_project_path");
        //     if (!dir_access.dir_exists(default_project_path)) {
        //         Error error = dir_access.make_dir_recursive(default_project_path);
        //         if (error != OK) {
        //             ERR_PRINT("Could not create default project directory at: " + default_project_path);
        //         }
        //     }

        //     String autoscan_path = EDITOR_GET("filesystem/directories/autoscan_project_path");
        //     if (!autoscan_path.is_empty()) {
        //         if (dir_access.dir_exists(autoscan_path)) {
        //             _scan_begin(autoscan_path);
        //         } else {
        //             Error error = dir_access.make_dir_recursive(autoscan_path);
        //             if (error != OK) {
        //                 ERR_PRINT("Could not create project autoscan directory at: " + autoscan_path);
        //             }
        //         }
        //     }

        //     SceneTree::get_singleton().get_root().connect("files_dropped", callable_mp(this, &ProjectManager::_files_dropped));
        #endregion TODO

        // Define a minimum window size to prevent UI elements from overlapping or being cut off.
        if (SceneTree.Singleton.Root is Window w)
        {
            w.MinSize = (new Vector2<float>(520, 350) * EditorScale.Value).As<int>();
        }

        // Resize the bootsplash window based on Editor display scale EditorScale.Value.
        var scaleFactor = MathF.Max(1, EditorScale.Value);
        if (scaleFactor > 1.0)
        {
            var windowSize = DisplayServer.Singleton.WindowGetSize();
            var screenRect = DisplayServer.Singleton.ScreenGetUsableRect(DisplayServer.Singleton.WindowGetCurrentScreen());

            windowSize = (windowSize.As<float>() * scaleFactor).As<int>();

            DisplayServer.Singleton.WindowSetSize(windowSize);

            if (screenRect.Size != default)
            {
                var window_position = new Vector2<int>
                (
                    screenRect.Position.X + (screenRect.Size.X - windowSize.X) / 2,
                    screenRect.Position.Y + (screenRect.Size.Y - windowSize.Y) / 2
                );

                DisplayServer.Singleton.WindowSetPosition(window_position);
            }
        }

        OS.Singleton.LowProcessorUsageMode = true;
    }

    private void DimWindow() => throw new NotImplementedException();

    private void OnSearchTermChanged(string term) => throw new NotImplementedException();
    private void OnTabChanged(int tab) => throw new NotImplementedException();

    private void ShowAbout() => throw new NotImplementedException();

    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
                {
                    #region TODO
                    // settings_hb->set_anchors_and_offsets_preset(Control::PRESET_TOP_RIGHT);
                    #endregion TODO
                    this.QueueRedraw();
                }
                break;

            case NotificationKind.NOTIFICATION_ENTER_TREE:
                {
                    #region TODO
                    // search_box->set_right_icon(get_theme_icon(SNAME("Search"), SNAME("EditorIcons")));
                    // search_box->set_clear_button_enabled(true);

                    // create_btn->set_icon(get_theme_icon(SNAME("Add"), SNAME("EditorIcons")));
                    // import_btn->set_icon(get_theme_icon(SNAME("Load"), SNAME("EditorIcons")));
                    // scan_btn->set_icon(get_theme_icon(SNAME("Search"), SNAME("EditorIcons")));
                    // open_btn->set_icon(get_theme_icon(SNAME("Edit"), SNAME("EditorIcons")));
                    // run_btn->set_icon(get_theme_icon(SNAME("Play"), SNAME("EditorIcons")));
                    // rename_btn->set_icon(get_theme_icon(SNAME("Rename"), SNAME("EditorIcons")));
                    // erase_btn->set_icon(get_theme_icon(SNAME("Remove"), SNAME("EditorIcons")));
                    // erase_missing_btn->set_icon(get_theme_icon(SNAME("Clear"), SNAME("EditorIcons")));
                    #endregion TODO

                    Engine.Singleton.EditorHint = false;
                }
                break;

            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
                {
                    #region TODO
                    // if (open_templates && open_templates->is_visible())
                    // {
                    //     open_templates->popup_centered();
                    // }
                    // if (asset_library)
                    // {
                    //     real_t size = get_size().x / EDSCALE;
                    //     // Adjust names of tabs to fit the new size.
                    //     if (size < 650)
                    //     {
                    //         local_projects_hb->set_name(TTR("Local"));
                    //         asset_library->set_name(TTR("Asset Library"));
                    //     }
                    //     else
                    //     {
                    //         local_projects_hb->set_name(TTR("Local Projects"));
                    //         asset_library->set_name(TTR("Asset Library Projects"));
                    //     }
                    // }
                    #endregion TODO
                }
                break;

            case NotificationKind.NOTIFICATION_READY:
                {
                    #region TODO
                    // int default_sorting = (int)EDITOR_GET("project_manager/sorting_order");
                    // filter_option->select(default_sorting);
                    // _project_list->set_order_option(default_sorting);

                    // #ifndef ANDROID_ENABLED
                    // if (_project_list->get_project_count() >= 1) {
                    //     // Focus on the search box immediately to allow the user
                    //     // to search without having to reach for their mouse
                    //     search_box->grab_focus();
                    // }
                    // #endif

                    // if (asset_library) {
                    //     // Removes extra border margins.
                    //     asset_library->add_theme_style_override("panel", memnew(StyleBoxEmpty));
                    //     // Suggest browsing asset library to get templates/demos.
                    //     if (open_templates && _project_list->get_project_count() == 0) {
                    //         open_templates->popup_centered();
                    //     }
                    // }
                    #endregion TODO
                }
                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_VISIBILITY_CHANGED:
                this.ProcessShortcutInput = this.IsVisibleInTree;
                break;

            case NotificationKind.NOTIFICATION_WM_CLOSE_REQUEST:
                this.DimWindow();
                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_WM_ABOUT:
                this.ShowAbout();
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
