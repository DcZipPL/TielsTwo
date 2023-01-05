# Cross-platform build script for Tiels
# This script builds the main C# program (Avalonia.Tiels) and Launcher/Updater application written in Rust
# it also downloads Lucide icon svg's from https://github.com/lucide-icons/lucide.

# Packages
import sys
import prepare

if prepare.check_buildtools():
    if sys.version_info[0] >= 3 and sys.version_info[1] >= 11:
        prepare.download_dependencies()
    else:
        print("Python <3.11 unsupported. Update your python interpreter!")
    prepare.create_output()