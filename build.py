# Cross-platform build script for Tiels
# This script builds the main C# program (Avalonia.Tiels) and Launcher/Updater application written in Rust
# it also downloads Lucide icon svg's from https://github.com/lucide-icons/lucide.
import prepare

if prepare.check_buildtools():
    prepare.download_dependencies()
    prepare.create_output()
    prepare.build(False)
