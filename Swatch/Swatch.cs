using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

using Swatch.Windows;

namespace Swatch;

public sealed class Swatch : IDalamudPlugin {
	[PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
	[PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
	[PluginService] internal static IPluginLog Log { get; private set; } = null!;
	private readonly WindowSystem WindowSystem = new("Swatch");

	private const string CommandName = "/swatch";

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


		Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
	}

	public void Dispose() {
		// Unregister all actions to not leak anything during disposal of plugin
		PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;
		PluginInterface.UiBuilder.OpenConfigUi -= this.ToggleConfigUi;

		this.WindowSystem.RemoveAllWindows();

		this.ConfigWindow.Dispose();

		CommandManager.RemoveHandler(CommandName);
	}

	private void OnCommand(string command, string args) => this.ToggleConfigUi();

	private void ToggleConfigUi() => this.ConfigWindow.Toggle();
}
