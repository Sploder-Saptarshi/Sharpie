.DEF MARIO_TOP_LEFT 0
.DEF MARIO_TOP_RIGHT 1
.DEF MARIO_BOT_LEFT 2
.DEF MARIO_BOT_RIGHT 3

.REGION FIXED

LDI r0, 14
LDI r1, 17
SWC r0, r1

LDI r0, 30
SWC r0, r1

LDI r0, 13
LDI r1, 31
SWC r0, r1

LDI r0, 29
SWC r0, r1

LDI r0, 18
LDI r1, 8
SWC r0, r1

CALL DrawMarioBro

Loop:
    LDI r0, 0
    INPUT r0, r1
    ICMP r1, 16
    JEQ CheckLuigi
    LDI r15, 0
    JMP CheckFireFlower

    CheckLuigi:
        CALL Increment15
        ICMP r15, 1
        JNE CheckFireFlower
        IXOR r3, 16
        CALL DrawMarioBro
        JMP Loop

    CheckFireFlower:
        ICMP r1, 32
        JEQ Increment14
        LDI r14, 0
        JMP Loop
        Increment14:
            INC r14
            ICMP r14, 1
            CALL SwapFireFlower
            JMP Loop


CALL DrawMarioBro
JMP Loop

Increment15:
    INC r15
    RET

DrawMarioBro:
    LDI r0, 0
    ALT CLS r0

    ICMP r3, 16 ; If we're using the alternative palette (aka if we are Luigi)
    JEQ MarioString

    LuigiString:
        .STR 6, 3 "Press A for Luigi"
        JMP Draw

    MarioString:
        .STR 6, 3 "Press A for Mario"

    Draw:
        LDI r0, 120
        LDI r1, 120
        LDI r2, MARIO_TOP_LEFT

        DRAW r0, r1, r2, r3
        IADD r0, 8
        INC r2

        DRAW r0, r1, r2, r3
        ISUB r0, 8
        IADD r1, 8
        INC r2

        DRAW r0, r1, r2, r3
        IADD r0, 8
        INC r2

        DRAW r0, r1, r2, r3
        VBLNK
    RET

SwapFireFlower:
    LDI r0, 2
    LDI r1, 1
    SWC r0, r1

    LDI r0, 18
    SWC r0, r1

    LDI r0, 15
    LDI r1, 2
    SWC r0, r1

    LDI r0, 31
    LDI r1, 8
    SWC r0, r1

    RET

.ENDREGION

.REGION SPRITE_ATLAS
.SPRITE 0
	.DB 0x00, 0x00, 0x22, 0x22
	.DB 0x00, 0x02, 0x22, 0x22
	.DB 0x00, 0x0D, 0xDD, 0xEE
	.DB 0x00, 0xDE, 0xDE, 0xEE
	.DB 0x00, 0xDE, 0xDD, 0xEE
	.DB 0x00, 0xDD, 0xEE, 0xEE
	.DB 0x00, 0x00, 0xEE, 0xEE
	.DB 0x00, 0x02, 0x2F, 0x22

.SPRITE 1
	.DB 0x20, 0x00, 0x00, 0x00
	.DB 0x22, 0x22, 0x00, 0x00
	.DB 0xDE, 0x00, 0x00, 0x00
	.DB 0xDE, 0xEE, 0x00, 0x00
	.DB 0xED, 0xEE, 0xE0, 0x00
	.DB 0xDD, 0xDD, 0x00, 0x00
	.DB 0xEE, 0xE0, 0x00, 0x00
	.DB 0x20, 0x00, 0x00, 0x00

.SPRITE 2
	.DB 0x00, 0x22, 0x2F, 0x22
	.DB 0x02, 0x22, 0x2F, 0xFF
	.DB 0x0E, 0xE2, 0xFB, 0xFF
	.DB 0x0E, 0xEE, 0xFF, 0xFF
	.DB 0x0E, 0xEF, 0xFF, 0xFF
	.DB 0x00, 0x0D, 0xFF, 0x00
	.DB 0x00, 0x22, 0x20, 0x00
	.DB 0x02, 0x22, 0x20, 0x00

.SPRITE 3
	.DB 0xF2, 0x22, 0x00, 0x00
	.DB 0xF2, 0x22, 0x20, 0x00
	.DB 0xBF, 0x2E, 0xE0, 0x00
	.DB 0xFF, 0xEE, 0xE0, 0x00
	.DB 0xFF, 0xFE, 0xE0, 0x00
	.DB 0xFF, 0xF0, 0x00, 0x00
	.DB 0x02, 0x22, 0x00, 0x00
	.DB 0x02, 0x22, 0x20, 0x00
.ENDREGION
