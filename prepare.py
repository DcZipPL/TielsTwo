import urllib.request
import urllib.error
import shutil
import os
import sys
from typing import final


ICONS_URL: final(str) = "https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/"
ICONS_PATH: final(str) = "./Avalonia.Tiels/Assets/Icons/out/"


def download_dependencies():
    # Download used icons. We don't want to download all, it will increase binary size.
    if sys.version_info[0] >= 3 and sys.version_info[1] >= 11:
        import tomllib
        print("Using `tomllib` for toml parsing.")
        with open("include.toml", "rb") as f:
            data = tomllib.load(f)
            for icon in data.get("icons"):
                try:
                    urllib.request.urlretrieve(ICONS_URL + icon + ".svg", ICONS_PATH + icon + ".svg")
                    print("Downloading " + icon + ".svg")
                except urllib.error.HTTPError:
                    print("Couldn't download " + icon + ".svg")
    else:
        print("Using outdated python version! Using `toml` package instead of standard `tomllib`")
        #import toml
        #toml.loads()

    # Other deps


def create_output():
    if not os.path.isdir("out"):
        os.mkdir("out")


def check_buildtools():
    print("Checking required buildtools...")
    missing_execs: int = 0
    missing_execs += int(not __check_buildtool("cargo"))
    missing_execs += int(not __check_buildtool("rustc"))
    missing_execs += int(not __check_buildtool("dotnet"))
    return missing_execs == 0


def __check_buildtool(tool: str):
    print(tool + "... " + str(shutil.which(tool) is not None))
    return shutil.which(tool) is not None
