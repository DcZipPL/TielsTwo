# Tiels II
Complete Rewrite of Tiels: An alternative for Fences.
With Tiels you can organize your desktop icons with tiles.

This rewrite focuses in Stability and making this program cross-platform.

## Building instructions
### Pre-requirements
Operating System:
- Linux 4.x.x+ or Windows 10+
- other systems ware not tested!

Installed tooling:
- dotnet
- cargo
- rustc
- python >= 3.11 or > 3.8 with [toml package](https://pypi.org/project/toml/)

### Building
#### Build script (Requires Python)
Linux / Windows:
```shell
git clone git@github.com:DcZipPL/TielsTwo.git
./build.py
```
All compiled binaries should be in `out` directory.
#### Manual (Python not required)
Linux / Windows:
```shell
# project isn't finished in most part. I don't want to change it every time when I add more dependencies. Currently use python script instead.
```
## Q&A
Q: What about old Tiels repository?
- A: Tiels I repository will be archived when this project will be stable.

Q: Will I keep my files after update?
- A: Yes. You will ONLY lose all theming of your tiles. There is plan for conversion tool, but I don't promise.

Q: When version 1.0.0 / When stable... ETA?
- A: Project is work in progress there is no ETA. But you can track progress [here (GitHub Projects)](https://github.com/users/DcZipPL/projects/1).
