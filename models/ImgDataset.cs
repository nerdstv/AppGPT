using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp3.models
{
    public class ImgDataset
    {
        public static string[] ImageRelatedWordsDataset = {
    "image", "photo", "picture", "photograph", "snapshot", "pic",    "graphic", "visual", "illustration", "drawing", "artwork",
    "photojournalism", "photomontage", "scenery", "landscape",    "portraiture", "capture", "shot", "still", "frame",
    "exposure", "focus", "composition", "lens", "shutter",    "camera", "photographer", "photography", "album", "gallery",
    "slideshow", "selfie", "filter", "editing", "retouching",    "imge", "fotograph", "pictur", "grapic", "vizual",
    "ilustration", "drawng", "artwrk", "phoojournalism", "photomontaj",
    "sceneryy", "landcape", "portaiture", "captur", "sot", "stil",
    "frmae", "exposur", "focs", "compositoin", "lns", "shuttr",
    "camra", "fotografer", "phography", "albom", "galery",
    "slidsow", "selfy", "filtar", "editng", "retoching",
    "img", "photto", "pictr", "gaphic", "visul", "snap",
    "ilustraion", "drwaing", "artwor", "photojournalim", "photmontage",
    "scennery", "landscap", "portraite", "capturre", "sot", "stll",
    "freme", "expousre", "foucs", "compsition", "lense", "shutterr",
    "cammera", "photografer", "phtography", "alubm", "galary",
    "slidehow", "selfiee", "fllter", "edting", "retocing",
    "imaj", "foto", "pix", "phograph", "snepshot",
    "illustartion", "darwing", "artwrok", "phojoournalism", "photomoontage",
    "scenneryy", "landdcape", "portraitture", "captture", "stil", "fram",
    "exposue", "foucs", "composittion", "lnse", "shuttter",
    "camerra", "fotoggrafer", "photogrpahy", "allbum", "gallerry",
    "slidshow", "selphie", "fiklter", "editinng", "retocshing",
    "imige", "phhotograph", "picturre", "grapik", "vissual",
    "ilustratoin", "drwaing", "artwokr", "photojournaism", "photomontagge",
    "sceenery", "landsacpe", "portaitue", "captur", "sstil", "frmaee",
    "exposuree", "focuss", "compositon", "lenss", "shuttterr",
    "camerra", "photographeer", "photogarphy", "allubm", "gallerey",
    "slidehoow", "slefie", "fillter", "editng", "retocing",
    "imig", "photho", "pictor", "grpahic", "visuall",
    "ilustrtion", "dawing", "artowrk", "photojouralism", "photomontag",
    "scenry", "landscpae", "potraiture", "captur", "stl", "framme",
    "exposre", "foucss", "composiiton", "lense", "shuttterr",
    "cammera", "photographeer", "photographhy", "albu", "gallry",
    "slideshoww", "selfee", "filterr", "edittng", "retohing"
    // Add more words and spelling errors here...
};

        public class ImageProcessing
        {
            public static string RemoveImageRelatedWords(string sentence)
            {
                string[] words = sentence.Split(' ');
                string modifiedSentence = "";

                foreach (string word in words)
                {
                    if (Array.IndexOf(ImgDataset.ImageRelatedWordsDataset, word.ToLower()) == -1)
                    {
                        modifiedSentence += word + " ";
                    }
                }

                // Remove the trailing space at the end of the sentence
                modifiedSentence = modifiedSentence.TrimEnd();

                return modifiedSentence;
            }
        }
    }
}
