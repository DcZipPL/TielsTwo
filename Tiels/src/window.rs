use std::process::exit;
use iced::{Alignment, Application, Command, Element, executor, Settings, Theme};
use iced::widget::{button, column, text};

pub(crate) fn create_window() -> iced::Result {
    ErrorWindow::run(Settings {
        flags: Flags{ title: "Error window".to_string() },
        ..Default::default()
    })
}

#[derive(Default)]
struct Flags {
    title: String,
}

struct ErrorWindow;

impl Application for ErrorWindow {
    type Executor = executor::Default;
    type Flags = Flags;
    type Message = ();
    type Theme = Theme;

    fn new(_flags: Flags) -> (ErrorWindow, Command<Self::Message>) {
        (ErrorWindow, Command::none())
    }

    fn title(&self) -> String {
        String::from("A cool application")
    }

    fn update(&mut self, _message: Self::Message) -> Command<Self::Message> {
        Command::none()
    }

    fn view(&self) -> Element<Self::Message> {
        "Hello, world!".into()
    }

    fn scale_factor(&self) -> f64 {
        0.8_f64
    }
}