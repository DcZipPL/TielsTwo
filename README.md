# Tiels II
Complete Rewrite of Tiels: An alternative for Fences.
With Tiels you can organize your desktop icons with tiles.

This rewrite focuses in Stability and making this program cross-platform.

## Building instructions
### Pre-requirements
Operating System (OR):
- Linux 4.x.x+
- Windows 10+
- or any operating system that works with Avalonia UI

Installed tooling (AND):
- dotnet
- cargo
- rustc

### Building

Linux:
```shell
git clone git@github.com:DcZipPL/TielsTwo.git
./build.sh
```
Windows (Powershell) {NOT TESTED}:
```batch
git clone git@github.com:DcZipPL/TielsTwo.git
md "out"
cd ./Tiels
cargo build --release
Copy-Item ./target/release/Tiels -Destination ../out
cd ../Avalonia.Tiels
dotnet build -o ../out
```
All compiled binaries should be in `out` directory.

## Q&A
Q: What with Tiels repository
- A: Tiels I repository will be archived when this project will be stable.

Q: Will I keep my files after update?
- A: Yes. You will loose only all theming of your tiles. There is plan for conversion tool but i don't promise.

Q: When version 1.0.0 / When stable... ETA?
- A: Project is work in progress there is no ETA. But you can track progress [here (Github Projects)](https://github.com/users/DcZipPL/projects/1).
