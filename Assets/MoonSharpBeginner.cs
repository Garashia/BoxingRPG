using MoonSharp.Interpreter;
using UnityEngine;

public class MoonSharpBeginner : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        double result = MoonSharpFactorial();
        Debug.Log("Lua execution result: " + result);
    }

    double MoonSharpFactorial()
    {
        string script = @"
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end

		return fact(5)";

        DynValue res = Script.RunString(script);
        return res.Number;
    }
}