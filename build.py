# Cross-platform build script for Tiels
# This script builds the main C# program (Avalonia.Tiels) and Launcher/Updater application written in Rust
# it also downloads Lucide icon svg's from https://github.com/lucide-icons/lucide.
import prepare
from datetime import datetime

if prepare.check_buildtools():
    start_time = datetime.now()
    prepare.download_dependencies()
    prepare.create_output()
    prepare.build(False)
    print("Build successfully in " + str(datetime.now() - start_time))
else:
    print("Missing dependencies. Please fix and rerun.")
