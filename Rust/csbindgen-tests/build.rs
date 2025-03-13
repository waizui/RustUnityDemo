use std::error::Error;
use std::fs;
use std::path::Path;

fn main() -> Result<(), Box<dyn Error>> {
    let cs_path = "../../Assets/Scripts/Gen/NativeMethods.cs";
    let lib_name = "csbindgenlib";

    csbindgen::Builder::default()
        .input_extern_file("src/lib.rs") // required
        .csharp_dll_name(lib_name) // required
        .csharp_class_name("NativeMethods") // optional, default: NativeMethods
        .csharp_namespace("CsBindgen") // optional, default: CsBindgen
        // .csharp_class_accessibility("internal") // optional, default: internal
        // .csharp_entry_point_prefix("") // optional, default: ""
        // .csharp_method_prefix("") // optional, default: ""
        // .csharp_use_function_pointer(true) // optional, default: true
        // .csharp_disable_emit_dll_name(false) // optional, default: false
        // .csharp_imported_namespaces("MyLib")    // optional, default: empty
        // .csharp_generate_const_filter (|_|false) // optional, default: `|_|false`
        // .csharp_dll_name_if("UNITY_IOS && !UNITY_EDITOR", "__Internal") // optional, default: ""
        // .csharp_type_rename(|rust_type_name| match rust_type_name {     // optional, default: `|x| x`
        //     "FfiConfiguration" => "Configuration".into(),
        //     _ => x,
        // })
        .generate_csharp_file(cs_path)?; // required

    let dest_bin_path = "../../Assets/Plugins/macOS/";
    fs::create_dir_all(dest_bin_path)?;

    let mac_src_path = format!("target/debug/lib{}.dylib", lib_name);
    if Path::new(&mac_src_path).exists() {
        let dest_file_path = format!("{}/{}.dylib", dest_bin_path, lib_name);
        fs::copy(mac_src_path, dest_file_path)?;
    } else {
        println!("source file {} not found", mac_src_path);
    }

    Ok(())
}
