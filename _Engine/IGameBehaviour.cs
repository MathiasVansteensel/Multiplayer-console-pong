using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine;
public interface IGameBehaviour
{
    bool IsMultiplayerEnabled { get; set; }
    string PlayerName { get; set; }
    string PlayerString { get; set; }
    abstract void Initialize();
    abstract void Update(float deltaTime);
    abstract void FixedUpdate(float deltaTime);
    abstract void UIUpdate(float deltaTime);
}
