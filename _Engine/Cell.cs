using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine;

public struct Cell
{
    public ConsoleColor Color { get; } = ConsoleColor.White;
    public char Character { get; } = ' ';

    public Cell(){}

    public Cell(char character) => Character = character;

    public Cell(char character, ConsoleColor color)
    {   
        Character = character;
        Color = color;
    }
}
