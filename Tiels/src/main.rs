use std::env;
use std::io::stdout;
use base64::Engine;
use { base64::engine::general_purpose::STANDARD as DECODER };
use crossterm::event::{Event, read};
use crossterm::ExecutableCommand;
use crossterm::style::{Color, Print, ResetColor, SetBackgroundColor, SetForegroundColor};

fn main() {
    let args = env::args().collect::<Vec<String>>();
    if args.len() > 1 {
        match args[1].as_str() {
            "update" => {},
            "error" => {
                if args.len() > 6 {
                    show_error(args);
                }
            },
            _ => {}
        }
    }
}

fn show_error(args : Vec<String>) -> anyhow::Result<()> {

    let error_type = String::from_utf8(DECODER.decode(&args[4])?)?;
    let source = String::from_utf8(DECODER.decode(&args[5])?)?;
    let warns = String::from_utf8(DECODER.decode(&args[6])?)?;
    let message = String::from_utf8(DECODER.decode(&args[2])?)?;
    let exception = String::from_utf8(DECODER.decode(&args[3])?)?;
    stdout()
        .execute(ResetColor)?
        .execute(Print("Tiels Error Handler\n\n"))?
        .execute(Print("Main application crashed!\n"))?
        .execute(Print("You see this window when something wrong happened or you have broke the config.\n"))?
        .execute(Print("If not then make issue on GitHub: "))?
        .execute(SetForegroundColor(Color::Cyan))?
        .execute(Print("https://github.com/DcZipPL/TielsTwo/issues\n"))?
        .execute(ResetColor)?
        .execute(Print("Type: "))?
        .execute(SetForegroundColor(if error_type == "FATAL" { Color::DarkRed } else { Color::Red }))?
        .execute(SetBackgroundColor(if error_type == "FATAL" { Color::Black } else { Color::Reset }))?
        .execute(Print(format!(" {} \n", error_type)))?
        .execute(ResetColor)?
        .execute(Print(format!("Source: {}\n", source)))?
        .execute(SetForegroundColor(Color::Yellow))?
        .execute(Print(format!("Warns: {}\n", warns)))?
        .execute(SetForegroundColor(Color::Red))?
        .execute(Print(format!("{}: \n", message)))?
        .execute(Print(format!("{}: \n", exception)))?
        .execute(ResetColor)?
        .execute(Print("Scroll up for more information.\n"))?
        .execute(Print("Press \"q\" to exit...\n"))?;

    loop {
        match read()? {
            Event::FocusGained => {}
            Event::FocusLost => {}
            Event::Key(key) => {
                if key.code == crossterm::event::KeyCode::Char('q') {
                    break;
                }
            }
            Event::Mouse(_) => {}
            Event::Paste(_) => {}
            Event::Resize(_, _) => {}
        }
    }

    Ok(())
}
