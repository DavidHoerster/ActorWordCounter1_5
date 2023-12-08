
using ActorMapReduceWordCount.Messages;

namespace ActorMapReduceWordCount.Actors;

public class WordReducerActor : ReceiveActor
{
    public static Props Create(int routeeCount)
    {
        return Props.Create(() => new WordReducerActor(routeeCount));
    }

    private Dictionary<String, Int32> _wordCount;
    private readonly int _routeeCount;
    private Int32 _completeRoutees;

    public WordReducerActor(int routeeCount)
    {
        _wordCount = new Dictionary<String, Int32>();
        _routeeCount = routeeCount;
        _completeRoutees = 0;
        SetupBehaviors();
    }

    private void SetupBehaviors()
    {
        Receive<MappedList>(msg =>
        {
            foreach (var key in msg.LineWordCount.Keys)
            {
                if (_wordCount.ContainsKey(key))
                {
                    _wordCount[key] += msg.LineWordCount[key];
                }
                else
                {
                    _wordCount.Add(key, msg.LineWordCount[key]);
                }
            }
        });

        Receive<Complete>(msg =>
        {
            _completeRoutees++;

            if (_completeRoutees == _routeeCount)
            {
                var topWords = _wordCount.OrderByDescending(w => w.Value).Take(25);
                foreach (var word in topWords)
                {
                    Console.WriteLine($"{word.Key} == {word.Value} times");
                }
            }
        });
    }
}