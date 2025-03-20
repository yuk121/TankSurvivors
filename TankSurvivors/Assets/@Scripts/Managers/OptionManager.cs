using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager 
{
    public string CreateUID()
    {
        StringBuilder uid = new StringBuilder();
        int rand = 0;

        for(int i =0; i < 15; i++)
        {
            if(i == 0)
            {
                rand = Random.Range(1, 10);
            }
            else
            {
                rand = Random.Range(0, 10);
            }

            uid.Append(rand.ToString());
        }

        return uid.ToString();
    }
}
