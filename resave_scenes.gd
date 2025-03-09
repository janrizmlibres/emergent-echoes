@tool # Makes this script run in the editor
extends EditorScript

func _run():
    var dir = DirAccess.open("res://")
    if dir:
        resave_scenes(dir)
    else:
        print("Failed to open res://")

func resave_scenes(dir: DirAccess):
    dir.list_dir_begin() # Start directory listing
    var file_name = dir.get_next()
    
    while file_name != "":
        if dir.current_is_dir():
            # Recursively process subdirectories
            var sub_dir = DirAccess.open(dir.get_current_dir() + "/" + file_name)
            if sub_dir:
                resave_scenes(sub_dir)
        elif file_name.ends_with(".tscn"):
            var full_path = dir.get_current_dir() + "/" + file_name
            print("Processing: ", full_path)
            # Load the scene
            var scene = load(full_path) as PackedScene
            if scene:
                # Optional: Modify the scene if needed (e.g., update RESET tracks)
                # Save it back
                var error = ResourceSaver.save(scene, full_path)
                if error == OK:
                    print("Saved: ", full_path)
                else:
                    print("Error saving ", full_path, ": ", error_string(error))
        file_name = dir.get_next()
    
    dir.list_dir_end()