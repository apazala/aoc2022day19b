using System.Text.RegularExpressions;

class State : IComparable<State>
{
    const int STATE_VARIABLES = 8;
    private int[] stateArray = new int[8];

    public void NextMinute()
    {
        Ore += OreRobots;
        Clay += ClayRobots;
        Obsidian += ObsidianRobots;
        Geode += GeodeRobots;
    }

    public int Ore { get { return stateArray[0]; } set { stateArray[0] = value; } }
    public int Clay { get { return stateArray[1]; } set { stateArray[1] = value; } }
    public int Obsidian { get { return stateArray[2]; } set { stateArray[2] = value; } }
    public int Geode { get { return stateArray[3]; } set { stateArray[3] = value; } }
    public int OreRobots { get { return stateArray[4]; } set { stateArray[4] = value; } }
    public int ClayRobots { get { return stateArray[5]; } set { stateArray[5] = value; } }
    public int ObsidianRobots { get { return stateArray[6]; } set { stateArray[6] = value; } }
    public int GeodeRobots { get { return stateArray[7]; } set { stateArray[7] = value; } }



    public State Clone()
    {
        State clone = new State();
        for (int i = 0; i < STATE_VARIABLES; i++)
            clone.stateArray[i] = stateArray[i];
        return clone;
    }

    public int CompareTo(State? other)
    {
        //Decreasing order or geodes: first = max
        int diff = other.Geode - Geode;
        if (diff == 0)
        {
            diff = other.GeodeRobots - GeodeRobots;
            if (diff == 0)
            {
                diff = other.Obsidian - Obsidian;
                if (diff == 0)
                {
                    diff = other.ObsidianRobots - ObsidianRobots;
                    if (diff == 0)
                    {
                        diff = other.Clay - Clay;
                        if (diff == 0)
                        {
                            diff = other.Ore - Ore;
                        }
                    }
                }
            }
        }


        return diff;
    }
}
class Blueprint
{
    static int instancesCounter = 0;

    public Blueprint()
    {
        ID = ++instancesCounter;
    }
    public int ID { get; }
    public int OreRobotOre { get; set; }
    public int ClayRobotOre { get; set; }
    public int ObsidianRobotOre { get; set; }
    public int ObsidianRobotClay { get; set; }
    public int GeodeRobotOre { get; set; }
    public int GeodeRobotObsidian { get; set; }

}
internal class Program
{
    private static State? OneMoreOreRobot(State state, Blueprint blueprint)
    {
        if (state.Ore < blueprint.OreRobotOre)
            return null;
        State newState = state.Clone();
        newState.OreRobots++;
        newState.Ore -= blueprint.OreRobotOre;
        newState.Ore--;
        return newState;
    }

    private static State? OneMoreClayRobot(State state, Blueprint blueprint)
    {
        if (state.Ore < blueprint.ClayRobotOre)
            return null;
        State newState = state.Clone();
        newState.ClayRobots++;
        newState.Ore -= blueprint.ClayRobotOre;
        newState.Clay--;
        return newState;
    }

    private static State? OneMoreObsidianRobot(State state, Blueprint blueprint)
    {
        if (state.Ore < blueprint.ObsidianRobotOre || state.Clay < blueprint.ObsidianRobotClay)
            return null;
        State newState = state.Clone();
        newState.ObsidianRobots++;
        newState.Ore -= blueprint.ObsidianRobotOre;
        newState.Clay -= blueprint.ObsidianRobotClay;
        newState.Obsidian--;
        return newState;
    }


    private static State? OneMoreGeodeRobot(State state, Blueprint blueprint)
    {
        if (state.Ore < blueprint.GeodeRobotOre || state.Obsidian < blueprint.GeodeRobotObsidian)
            return null;

        State newState = state.Clone();
        newState.GeodeRobots++;
        newState.Ore -= blueprint.GeodeRobotOre;
        newState.Obsidian -= blueprint.GeodeRobotObsidian;
        newState.Geode--;
        return newState;

    }

    private static void Main(string[] args)
    {
        List<Blueprint> blueprints = new List<Blueprint>();

        Regex rx = new Regex(@"^Blueprint \d+: Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.",
          RegexOptions.Compiled);
        string[] lines = File.ReadAllLines(@"input.txt");
        foreach (string line in lines)
        {
            GroupCollection groups = rx.Match(line).Groups;
            blueprints.Add(new Blueprint
            {
                OreRobotOre = int.Parse(groups[1].Value),
                ClayRobotOre = int.Parse(groups[2].Value),
                ObsidianRobotOre = int.Parse(groups[3].Value),
                ObsidianRobotClay = int.Parse(groups[4].Value),
                GeodeRobotOre = int.Parse(groups[5].Value),
                GeodeRobotObsidian = int.Parse(groups[6].Value)
            });

            if(blueprints.Count == 3) break;
        }

        List<State> previousStates, currentStates;
        currentStates = new List<State>();

        long ans = 1;
        foreach (Blueprint blueprint in blueprints)
        {
            currentStates.Clear();
            currentStates.Add(new State { OreRobots = 1 });
            for (int i = 0; i < 32; i++)
            {
                //Console.WriteLine($"{i} => ");
                previousStates = currentStates.GetRange(0, Math.Min(100000, currentStates.Count));
                currentStates = new List<State>();
                foreach (State state in previousStates)
                {
                    State? newState = OneMoreOreRobot(state, blueprint);
                    if (newState != null)
                        currentStates.Add(newState);

                    newState = OneMoreClayRobot(state, blueprint);
                    if (newState != null)
                        currentStates.Add(newState);

                    newState = OneMoreObsidianRobot(state, blueprint);
                    if (newState != null)
                        currentStates.Add(newState);

                    newState = OneMoreGeodeRobot(state, blueprint);
                    if (newState != null)
                        currentStates.Add(newState);

                    currentStates.Add(state);

                }
                foreach (State state in currentStates)
                    state.NextMinute();

                currentStates.Sort();
            }

            Console.WriteLine(currentStates[0].Geode);
            ans *= currentStates[0].Geode;
        }

        Console.WriteLine($"ANS: {ans}");
    }
}