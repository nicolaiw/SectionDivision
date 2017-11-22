using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etappen
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<int> { 10, 3, 4, 11, 6, 8 };
            var res = list[BinarySearch(list, 8)];

            Action<string> print = Console.WriteLine;

            // Daten einlesen
            print("Bitte Anzahl der Etappen eingeben: ");
            var numbSections = int.Parse(Console.ReadLine());
            print("Bitte Anzahl der Tage eingeben: ");
            var numbDays = int.Parse(Console.ReadLine());

            print("Bitte die Etappen zeilenweise eingeben: ");
            var sectionlist = new List<int>();

            for (int i = 0; i < numbSections; i++)
            {
                sectionlist.Add(int.Parse(Console.ReadLine()));
            }

            /*
                Aufgabenstellung:
                
                Die MAXIMALE Tagesstrecke soll MINIMIERT werden.
                Der minimal möglichste Wert ist nun der, wenn die maximale
                Tagesstrecke auf einen Tage gelegt wird.

                Beispiel: |----5----|----5----|----5----|----6----|

                Klar wird dies wenn Anzahl Tage = Anzahl Etappen setzt.
                Hier also 4. Die maximale Tagesstrecke minimiert ergibt somit 6.
                Sobald Anzahl Tage < Anzahl Etappen, ist die maximale Tagesstrecke minimiert > 6.

                Der schlechteste Falle ergibt sich dann, wenn man die Anzahl der Tage auf 1 festlegt.
                Dann ist die maximale Tagesstrecke minimiert gleich die Summe aller Etappen, also

                3*5 + 6 = 21

                Bis hier hin kurz zusammengefasst:

                    Maximale Tagesstrecke minimiert optimum = 6     // folglich opt genannt
                    Maximale Tagesstrecke minimiert worst case = 21 // folglich worst genannt

                Mit der binären Suche lässt sich nun für gegebenen Etappen und gegebenen Anzahl an Tagen
                das Ergebnis für die maximale Tagesstrecke minimiert ermitteln, indem man für den Startwert
                opt und für den Endwert worst nutzt.

                Hinweise: Die binäre suche verlangt normalerweise eine Liste in geordneter Reihenfolge.
                          Da die Reihenfolge der Etappen jedoch nicht verändert werden darf, muss der
                          Algorithmus dahingehend angepasst werden.

                
                Rechenbeispiel:

                    Gegeben: 
                    
                    Etappen: |----11----|----16----|----5----|----5----|----12----|----10----|
                    Tage: 3

                    solange (opt <= worst):
                    |
                    |    mittelwert: (opt + worst) / 2
                    |    zwischensummer: 0
                    |    anzahlTagesstrecken: 1
                    |
                    |    für alle: Etappen et
                    |    |    
                    |    |    wenn (zwischenSumme + et > mittelwert)
                    |    |        zwischensummer: et
                    |    |        anzahlTagesstrecken: anzahlTagesstrecken + 1
                    |    |    sonst
                    |    |        zwischensumme: zwischensumme + et
                    |    
                    |    wenn (anzahlTagesstrecken <= Tage)
                    |        worst: mittelwert - 1 // Das Optimum KANN, MUSS aber nicht gefunden worden sein (Stichwort: Greedy-Algorithmus)
                    |    sonst
                    |        opt: mittelwert + 1   // Aufteilung Ungültig, das optimum muss höher liegen.

                    An dieser Stelle steht in opt das minimierte Maximum.

                    Schleifendurchgang      opt     worst   mittelwert
                    1                       16      59      37
                    2                       16      36      26
                    3                       16      25      20
                    4                       21      25      23
                    5                       24      25      24
                    6                       25      25      25

                    Da am Ende der Schleife opt + 1 --> opt = 26
                    

                    Nun müssen noch die übrigen Teilstrecken Berechnet werden:

                    tag: 1
                    tagessumme: 0

                    für alle: Etappen et
                    |   wenn (tagessumm + et > opt)
                    |       Ausgabe: t
                    |       tagessumme: et
                    |       tag: tag: tag +1
                    |   sonst
                    |       tagessumme: tagessumme + ep

                    Schleifendurchgang      tag     tagessumme      et
                    1                       1       0               11
                    2                       1       11              16 (tagessumme + et > mittelwert) --> 11 ist tag 1
                    3                       2       16              5
                    4                       2       21              5
                    5                       2       26              12 (tagessumme + et > mittelwert) --> 26 ist tag 2
                    6                       3       12              10

                    --> da wir bei tag 3 sind --> rest der teilstrecken ist letzter tag --> 12 + 10 --> 22 ist tag 3

                    Result: Tag 1: 11
                            Tag 2: 26
                            Tag 3: 22
            */

            // Binärsuche
            // Optimierungen noch möglich (die forschleife kann unter Umständen vorzeitig
            // verlassen werden.
            // Aufgrund der Lesbarkeit jedoch wie folgt.
            var minimal = sectionlist.Max();
            var maximal = sectionlist.Sum();

            while (minimal <= maximal)
            {
                var mid = (minimal + maximal) / 2;
                var subSum = 0;
                var numbSubLists = 1;

                foreach (var sec in sectionlist)
                {
                    if (subSum + sec > mid)
                    {
                        subSum = sec;
                        numbSubLists += 1;
                    }
                    else
                        subSum += sec;
                }

                if (numbSubLists <= numbDays)
                    maximal = mid - 1;
                else
                    minimal = mid + 1;
            }

            // ---> Result is in minimal
            print("Die maximale Tagesstrecke der optimalen Lösung: " + minimal);

            var tag = 1;
            var tagessumme = sectionlist.First();

            foreach (var sec in sectionlist.Skip(1))
            {
                if (tagessumme + sec > minimal)
                {
                    print("Tag " + tag + ": " + tagessumme);
                    tagessumme = sec;
                    tag += 1;
                }
                else
                {
                    tagessumme += sec;
                }
            }

            // Letzter Tag
            print("Tag " + tag + ": " + tagessumme);

            Console.ReadLine();
        }


        static void log(int day, int sum)
        {
            Action<string> print = Console.WriteLine;
            print("Tag " + day + ": " + sum);
        }

        static List<int> CalcSteps(List<int> sectionlist, int minimal)
        {
            var res = new List<int>();
            var tag = 1;
            var tagessumme = 0;

            foreach (var sec in sectionlist)
            {
                if (tagessumme + sec > minimal)
                {
                    log(tag, tagessumme);

                    res.Add(tagessumme);

                    tagessumme = sec;
                    tag += 1;
                }
                else
                {
                    tagessumme += sec;
                }
            }

            res.Add(tagessumme);
            log(tag, tagessumme);

            return res;
        }

        // A simple BinarySearch
        static int BinarySearch(List<int> list, int searchFor)
        {
            var sortedList = list.OrderBy(i => i).AsEnumerable().ToList();
           
            var left = 0;
            var right = sortedList.Count() - 1;

            while(left <= right)
            {
                var mid = (left + right) / 2;

                if (sortedList[mid] == searchFor)
                    return mid;
                else if (sortedList[mid] > searchFor)
                    right = mid - 1;
                else
                    left = mid + 1;
            }

            return -1;
        }
    }
}
