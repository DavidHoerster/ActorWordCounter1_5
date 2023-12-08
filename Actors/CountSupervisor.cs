using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using ActorMapReduceWordCount.Messages;
using System.Diagnostics;

namespace ActorMapReduceWordCount.Actors
{
    public class CountSupervisor : ReceiveActor
    {
        public static Props Create()
        {
            return Props.Create(() => new CountSupervisor());
        }

        private Dictionary<String, Int32> _wordCount;
        private readonly Int32 _numberOfRoutees;
        private Int32 _completeRoutees;

        public CountSupervisor()
        {
            _wordCount = new Dictionary<String, Int32>();
            _numberOfRoutees = 5;
            _completeRoutees = 0;

            SetupBehaviors();
        }

        private void SetupBehaviors()
        {
            Receive<StartCount>(msg =>
            {
                var fileInfo = new FileInfo(msg.FileName);
                var lineNumber = 0;

                var lineReader = Context.ActorOf(new RoundRobinPool(_numberOfRoutees)
                                    .Props(LineReaderActor.Create()));

                using (var reader = fileInfo.OpenText())
                {
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;

                        var line = reader.ReadLine() ?? String.Empty;
                        lineReader.Tell(new ReadLineForCounting(lineNumber, line));
                    }
                }

                lineReader.Tell(new Broadcast(new Complete()));
            });

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

                if (_completeRoutees == _numberOfRoutees)
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
}