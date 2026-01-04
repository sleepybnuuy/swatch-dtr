using System;
using System.Globalization;

using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

using Swatch.Windows;

namespace Swatch;

public sealed class Swatch : IDalamudPlugin {
	[PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
	[PluginService] internal static IFramework Framework { get; private set; } = null!;
	[PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
	[PluginService] internal static IPluginLog Log { get; private set; } = null!;
	[PluginService] internal static IDtrBar DtrBar { get; private set; } = null!;

	private readonly WindowSystem WindowSystem = new("Swatch");

	private IDtrBarEntry _dtrEntry;

	private const string CommandName = "/swatch";
	private const string DtrTitle = "Swatch";

	public Configuration Configuration { get; init; }

	private ConfigWindow ConfigWindow { get; init; }

	public Swatch() {
		this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

		this.ConfigWindow = new ConfigWindow(this);
		this.WindowSystem.AddWindow(this.ConfigWindow);
		PluginInterface.UiBuilder.Draw += this.WindowSystem.Draw;
		PluginInterface.UiBuilder.OpenConfigUi += this.ToggleConfigUi;

		CommandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand) {
			HelpMessage = "open swatch-dtr config"
		});

		this._dtrEntry = DtrBar.Get(DtrTitle);
		this._dtrEntry.Text = "the swatcherrrr";
		this._dtrEntry.Tooltip = ".beat time";
		this._dtrEntry.OnClick = args => this.ConfigWindow.Toggle();

		Framework.Update += this.SwatchUpdate;

		Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
	}

	private void SwatchUpdate(IFramework _) {
		var time = DateTime.UtcNow;
		time += TimeSpan.FromHours(1);
		var ms = ((time.Hour * 60 + time.Minute) * 60 + time.Second) * 1000 + time.Millisecond;
		var beats = Math.Floor((double)Math.Abs(ms / 86400));

		var label = "";
		if (this.Configuration.ShowInternetLabel)
			label += "internet time ";
		label += "@ " + beats.ToString(CultureInfo.InvariantCulture);

		this._dtrEntry.Text = label;
	}

	public void Dispose() {
		// Unregister all actions to not leak anything during disposal of plugin
		PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;
		PluginInterface.UiBuilder.OpenConfigUi -= this.ToggleConfigUi;

		this.WindowSystem.RemoveAllWindows();
		this.ConfigWindow.Dispose();

		this._dtrEntry.Remove();

		CommandManager.RemoveHandler(CommandName);
	}

	private void OnCommand(string command, string args) => this.ToggleConfigUi();

	private void ToggleConfigUi() => this.ConfigWindow.Toggle();
}
