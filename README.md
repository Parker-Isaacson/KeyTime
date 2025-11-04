# KeyTime

KeyTime is a timeline based macro program. It permits the user to create macros that run on multiple threads.

### Langauge

KeyTime comes with an inbuild language, keytime.

```keytime
; Comments start with ';'.

; The commands that are valid are as follows, capitalization does not matter.
; press <key(s)> ; Keys can be a list of keys like KEY and keys 'k', 'e', and 'y' will be pressed.
; unpress <key(s)> ; Keys can be a list of keys like KEY and keys 'k', 'e', and 'y' will be unpressed.
; tap <key(s)> ; Keys can be a list of keys like KEY and keys 'k', 'e', and 'y' will be tapped, in order.
; sleep <time> ; The time value is in milliseconds.
; macro <id> ; Defines a new macro.

; Keys that are still pressed at the end of a macro will be unpressed automatically.

macro 0
 press KEY
 sleep 500
 unpress K
 tap Q
	; Keys 'e' and 'y' are automatically unpressed.
```

Variables have the following syntax and can only be used with `sleep`.

```keytime
time: 500 ; Create a variable named 'time' with value 500. The value is accessed via $time
; Defines macro 0, which will press 'K' for 500ms.
macro 0
 press K
 sleep $time
 unpress K
```

### Behind-The-Scenes

Threads and there respective list of commands are assigned a 32 bit number as for very fast lookup via a Dictionary. The top 6 bits are reserved for the thread number, the remaining 26 are reserved for the command index.