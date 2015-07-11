using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class AgentManager : MonoBehaviour {

    private List<Agent> agents = new List<Agent>();

    private Agent current;

	// Use this for initialization
	void Start () {
	    // Load the data
        Load();
	}

    public void Load() {
        // Load the data file
        TextAsset test = (TextAsset)Resources.Load("data", typeof(TextAsset));
        string[] lines = test.text.Split('\n');

        // Load each agent
        for (int i = 1; i < lines.Length; i++)
            if(lines[i].Length >= 2)
                agents.Add(new Agent(lines[i]));
    }

    public bool IsAgentWithCode(string code) {
        // Trim the code
        code.Trim();

        // Check whether there is an agent with this code
        foreach (Agent a in agents)
            if (a.GetCode().Equals(code))
                return true;
        return false;
    }

    public Agent GetAgentWithCode(string code) {
        // Trim the code
        code.Trim();

        // Check whether there is an agent with this code
        foreach(Agent a in agents)
            if(a.GetCode().Equals(code))
                return a;
        return null;
    }

    public Agent GetCurrentAgent() {
        return this.current;
    }

    public void SetCurrentAgent(Agent agent) {
        this.current = agent;
    }

    public void SetCurrentAgent(string code) {
        this.current = GetAgentWithCode(code);
    }
}
