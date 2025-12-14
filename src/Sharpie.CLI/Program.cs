// See https://aka.ms/new-console-template for more information
using Sharpie.Core;

Console.WriteLine("Booting Sharpie...");

var memory = new Memory();
var cpu = new Cpu(memory);

memory.WriteByte(0x0000, 0x00); // NOP
memory.WriteByte(0x0001, 0x00); // NOP
memory.WriteByte(0x0002, 0xFF); // HALT

while (!cpu.IsHalted)
{
    Console.WriteLine(cpu.ToString());
    cpu.Cycle();

    Thread.Sleep(100);
}

Console.WriteLine("Cpu halted successfully.");