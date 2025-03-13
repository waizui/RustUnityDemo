use core::slice;
use std::os::raw::c_float;

//lib.rs, simple FFI code
#[no_mangle]
pub extern "C" fn my_add(x: i32, y: i32) -> i32 {
    x + y
}

#[no_mangle]
pub extern "C" fn process_float_arr(floats_ptr: *mut c_float, count: i32, time: f32) {
    let floats = unsafe {
        if floats_ptr.is_null() || count <= 0 {
            return;
        }
        slice::from_raw_parts_mut(floats_ptr, count as usize)
    };

    for i in 0..(floats.len()) {
        if i % 3 == 0 {
            floats[i] += 0.01 * time.sin();
        }
    }
}
