extends Node

const LOG_DIR = "user://logs/"

# File object
var log_file = null
var current_log_path = ""

func _ready():
    # Ensure the logs directory exists
    var dir = DirAccess.open("user://")
    if dir and not dir.dir_exists(LOG_DIR):
        dir.make_dir_recursive(LOG_DIR)
    
    # Create a unique filename with timestamp
    var timestamp = Time.get_datetime_string_from_system().replace(":", "-") # Replace : with - for valid filename
    current_log_path = LOG_DIR + "game_log_%s.txt" % timestamp

    var file = FileAccess.open(current_log_path, FileAccess.WRITE)
    if file:
        log_file = file
        custom_log("Game started at: " + Time.get_datetime_string_from_system())
    else:
        print("Failed to open log file: " + current_log_path)

func _exit_tree():
    if log_file and log_file.is_open():
        custom_log("Game closed at: " + Time.get_datetime_string_from_system())
        log_file.close()

# Main logging function
func custom_log(message: String):
    if log_file and log_file.is_open():
        # Format with timestamp
        var timestamp = Time.get_datetime_string_from_system()
        var formatted_message = "[%s] %s" % [timestamp, message]
        log_file.store_line(formatted_message)
        log_file.flush()
        # Optional: Also print to console for debugging
        print(formatted_message)
    else:
        # Fallback to print if file isn't available
        print("[ERROR] Log file not available: " + message)

# Different log levels
func info(message: String):
    custom_log("[INFO] " + message)

func warning(message: String):
    custom_log("[WARNING] " + message)

func error(message: String):
    custom_log("[ERROR] " + message)