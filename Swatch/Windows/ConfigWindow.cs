using System;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace Swatch.Windows;

public class ConfigWindow : Window, IDisposable {
	private readonly Configuration configuration;
	public ConfigWindow(Swatch plugin) : base("Swatch Config") {
		this.configuration = plugin.Configuration;
	}

	public void Dispose() { }

	public override void Draw() {
		var updated = false;
		ImGui.TextWrapped("Swatch is a DTR-bar plugin to show the current time in Swatch's beats format, inspired by Phantasy Star Online.");
		ImGui.TextWrapped("You can edit the ordering of your DTR bar in Dalamud's plugin configs.");

		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();

		var showInternet = this.configuration.ShowInternetLabel;
		if (ImGui.Checkbox("Show 'internet time' label", ref showInternet)) {
			this.configuration.ShowInternetLabel = showInternet;
			updated = true;
		}

		if (updated)
			this.configuration.Save();
	}
}
