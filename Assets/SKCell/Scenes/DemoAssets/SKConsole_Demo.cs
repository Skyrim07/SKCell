using UnityEngine;

namespace SKCell.Test
{
    public class SKConsole_Demo : MonoBehaviour
    {
        void Start()
        {
            SKConsoleCommand p = 
            SKConsole.AddCommand("print", "Print a number", (x) =>
            {
                print(x);
            });
            p = SKConsole.AddCommand("add", "Add and print two numbers", (x, y) =>
            {
                print($"{x} + {y} = {x+y}");
            });
            p = SKConsole.AddCommand("mul3", "Multiply and print three numbers", (x, y, z) =>
            {
                print($"{x} * {y} * {z} = {x * y * z}");
            });
            p = SKConsole.AddCommand("set", "Set a variable");
            p.AddCommand("var_a", "Set variable A", (x) =>
            {
                print($"Set A = {x}");
            });
            p.AddCommand("var_b", "Set variable B", (x) =>
            {
                print($"Set B = {x}");
            });
            SKConsoleCommand cmd2 = SKConsole.AddCommand("loadscene", " ");
            for (int i = 0; i < 10; i++)
            {
                int ii = i;
                cmd2.AddCommand($"{100+ii}", $"Load scene {100+ii}", () =>
                {
                    print(100+ii);
                });
            }

            SKConsole.AddCommand("addhp", "Add HP to the current player.", (x) =>
            {
                print(x);
            });
            SKConsole.AddCommand("subhp", "Subtract HP to the current player.", (x) =>
            {
                print(x);
            });
            SKConsole.Open();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SKConsole.Toggle();
            }
        }

    }
}