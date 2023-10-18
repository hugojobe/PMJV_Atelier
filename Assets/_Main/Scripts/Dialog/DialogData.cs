using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogData
{
    public List<DialogSegment> segments;
    public const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

    public DialogData(string rawDialog){
        segments = RipSegments(rawDialog);
    }

    public List<DialogSegment> RipSegments(string rawDialog){
        List<DialogSegment> segments = new List<DialogSegment>();
        MatchCollection matches = Regex.Matches(rawDialog, segmentIdentifierPattern);

        int lastIndex = 0;

        DialogSegment segment = new DialogSegment();
        segment.dialog = (matches.Count == 0)? rawDialog : rawDialog.Substring(0, matches[0].Index);
        segment.startSignal = DialogSegment.StartSignal.NONE;
        segment.signalDelay = 0;
        segments.Add(segment);

        if(matches.Count == 0){
            return segments;
        } else {
            lastIndex = matches[0].Index;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            segment = new DialogSegment();

            //Récupérer le signal de départ pour le segment
            string signalMatch = match.Value;
            signalMatch = signalMatch.Substring(1, match.Length - 2);
            string[] signalSplit = signalMatch.Split(' ');

            segment.startSignal = (DialogSegment.StartSignal) Enum.Parse(typeof(DialogSegment.StartSignal), signalSplit[0].ToUpper());

            // Récupérer le délai (wc et wa)
            if(signalSplit.Length > 1){
                float.TryParse(signalSplit[1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out segment.signalDelay);
            }

            //Récupérer le dialogue du segments
            int nextIndex = i + 1 < matches.Count? matches[i+1].Index : rawDialog.Length;
            segment.dialog = rawDialog.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
            lastIndex = nextIndex;

            segments.Add(segment);
        }

        return segments;
    }

    public struct DialogSegment{
        public string dialog;
        public StartSignal startSignal;
        public float signalDelay;


        public enum StartSignal{NONE, C, A, WA, WC}

        public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
    }
}
