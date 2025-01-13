using Febucci.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Febucci.UI
{
    /// <summary>
    /// Built-in typewriter, which shows letters dynamically character after character.<br/>
    /// To enable it, add this component near a <see cref="TAnimCore"/> one<br/>
    /// - Base class: <see cref="TypewriterCore"/><br/>
    /// - Manual: <see href="https://www.febucci.com/text-animator-unity/docs/typewriters/">TextAnimatorPlayers</see>
    /// </summary>
    [HelpURL("https://www.febucci.com/text-animator-unity/docs/typewriters/")]
    [AddComponentMenu("Febucci/TextAnimator/Typewriter - By Character")]
    public class CustomTypewriter : Core.TypewriterCore
    {
        [SerializeField, Attributes.CharsDisplayTime, Tooltip("Wait time for normal letters")] public float waitForNormalChars = .03f;
        [SerializeField, Attributes.CharsDisplayTime, Tooltip("Wait time for ! ? .")] public float waitLong = .6f;
        [SerializeField, Attributes.CharsDisplayTime, Tooltip("Wait time for ; : ) - ,")] public float waitMiddle = .2f;

        [System.Obsolete("Typo, please use 'avoidMultiplePunctuationWait' instead.")]
        public bool avoidMultiplePunctuactionWait => avoidMultiplePunctuationWait;
        
        [FormerlySerializedAs("avoidMultiplePunctuactionWait")]
        [SerializeField, Tooltip("-True: only the last punctuation on a sequence waits for its category time.\n-False: each punctuation will wait, regardless if it's in a sequence or not")] public bool avoidMultiplePunctuationWait = false;

        [SerializeField, Tooltip("True if you want the typewriter to wait for new line characters")] public bool waitForNewLines = true;

        [SerializeField, Tooltip("True if you want the typewriter to wait for all characters, false if you want to skip waiting for the last one")] public bool waitForLastCharacter = true;

        [SerializeField, Tooltip("True if you want to use the same typewriter's wait times for the disappearance progression, false if you want to use a different wait time")] public bool useTypewriterWaitForDisappearances = true;
        [SerializeField, Attributes.CharsDisplayTime, Tooltip("Wait time for characters in the disappearance progression")] float disappearanceWaitTime = .015f;
        [SerializeField, Attributes.MinValue(0.1f), Tooltip("How much faster/slower is the disappearance progression compared to the typewriter's typing speed")] public float disappearanceSpeedMultiplier = 1;

        [SerializeField, Tooltip("Maximum number of characters before clearing the text")]
        private int maxCharactersInRect = 40; // Adjust this value as needed

        private int currentCharacterCount = 0;

        private TextMeshProUGUI _textMeshPro;

        private void Awake()    {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }


        protected override float GetWaitAppearanceTimeOf(int charIndex)
        {

            char character = TextAnimator.Characters[charIndex].info.character;
            
            //avoids waiting for the last character
            if (!waitForLastCharacter && TextAnimator.allLettersShown)
                return 0;

            //avoids waiting for multiple times if there are puntuactions near each other
            if (avoidMultiplePunctuationWait && char.IsPunctuation(character)) //curr char is punctuation
            {
                //next char is punctuation too, so skips this one
                if (charIndex < TextAnimator.CharactersCount - 1
                    && char.IsPunctuation(TextAnimator.Characters[charIndex + 1].info
                        .character))
                {
                    return waitForNormalChars;
                }
            }

            //avoids waiting for new lines
            if (!waitForNewLines && !TextAnimator.latestCharacterShown.info.isRendered)
            {
                bool IsUnicodeNewLine(ulong unicode) //Returns true if the unicode value represents a new line
                {
                    return unicode == 10 || unicode == 13;
                }

                //skips waiting for a new line
                if (IsUnicodeNewLine(System.Convert.ToUInt64(TextAnimator.latestCharacterShown.info.character)))
                    return 0; //TODO test
            }

            //character is not before another punctuaction
            switch (character)
            {
                case ';':
                case ':':
                case ')':
                case '-':
                case ',': return waitMiddle;

                case '!':
                case '?':
                case '.':
                    return waitLong;
            }


            if (currentCharacterCount >= maxCharactersInRect)
            {
                // Check if the next character is a space or punctuation, indicating the end of a word
                if (charIndex < TextAnimator.CharactersCount - 1)
                {
                    char nextCharacter = TextAnimator.Characters[charIndex + 1].info.character;
                    if (char.IsWhiteSpace(nextCharacter))
                    {
                        ClearText();
                        currentCharacterCount = 0;
                    }
                }
            }

            currentCharacterCount++;

            if(currentCharacterCount == TextAnimator.CharactersCount){
                currentCharacterCount = 0;
                StartCoroutine(ClearTextAfterDelay());
            }

            return waitForNormalChars;
        }

        private void ClearText()
        {
            // Remove the current appeared text efficiently
            _textMeshPro.text = _textMeshPro.text.Replace("<noparse>", "").Replace("</noparse>", "")[currentCharacterCount..];


            // Reset the character count
            currentCharacterCount = 0;
        }

        protected override float GetWaitDisappearanceTimeOf(int charIndex)
        { 
            return useTypewriterWaitForDisappearances ? GetWaitAppearanceTimeOf(charIndex) * (1/disappearanceSpeedMultiplier) : disappearanceWaitTime;
        }

        IEnumerator ClearTextAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            _textMeshPro.text = "";
        }

    }
}