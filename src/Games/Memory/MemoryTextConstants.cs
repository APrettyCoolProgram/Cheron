// 261030_code
// 261030_documentation

namespace Cheron.Games.Memory;

/// <summary>Centralizes fixed label and help text used by the Memory toolbar, footer, and overlays.</summary>
internal static class MemoryTextConstants
{
    /// <summary>The toolbar title label.</summary>
    public const string TitleLabel = "MEMORY";

    /// <summary>The New Game toolbar button label.</summary>
    public const string NewGameLabel = "NEW GAME";

    /// <summary>The Help toolbar button label.</summary>
    public const string HelpLabel = "HELP";

    /// <summary>The Sound toolbar button label prefix, followed by ON/OFF.</summary>
    public const string SoundLabel = "SOUND";

    /// <summary>The label shown on the Sound button when audio could not be initialized.</summary>
    public const string AudioUnavailableLabel = "NO AUDIO";

    /// <summary>The footer instruction shown centered in the bottom strip.</summary>
    public const string FooterFlipInstructionLabel = "ARROWS + ENTER  FLIP";

    /// <summary>The headline shown on the board-size picker prompt.</summary>
    public const string SizePromptTitleLabel = "CHOOSE BOARD SIZE";

    /// <summary>The instruction shown at the bottom of the board-size picker prompt.</summary>
    public const string SizePromptHintLabel = "ARROWS TO SELECT, ENTER TO CONFIRM, ESC TO CANCEL";

    /// <summary>The confirm action label on the board-size picker prompt.</summary>
    public const string SizePromptConfirmLabel = "START";

    /// <summary>The cancel action label on the board-size picker prompt.</summary>
    public const string SizePromptCancelLabel = "CANCEL";

    /// <summary>The moves statistic caption, followed by the current count.</summary>
    public const string MovesCaptionLabel = "MOVES";

    /// <summary>The time statistic caption, followed by the elapsed mm:ss.</summary>
    public const string TimeCaptionLabel = "TIME";

    /// <summary>The headline shown on the win celebration overlay.</summary>
    public const string WinMessageLabel = "YOU WIN!";

    /// <summary>The instruction shown beneath the win celebration headline.</summary>
    public const string WinSubMessageLabel = "PRESS N FOR A NEW GAME";

    /// <summary>The headline shown on the help overlay.</summary>
    public const string HelpTitleLabel = "HOW TO PLAY";

    /// <summary>The help overlay body lines, drawn in order.</summary>
    public static readonly string[] HelpLines =
    [
        "CLICK A TILE, OR MOVE FOCUS WITH THE ARROW KEYS",
        "PRESS ENTER OR SPACE TO FLIP THE FOCUSED TILE",
        "FLIP TWO TILES TO CHECK FOR A MATCHING COLOR",
        "MATCHED PAIRS STAY FACE UP",
        "MATCH EVERY PAIR TO WIN THE GAME",
        "",
        "N   CHOOSE A BOARD SIZE AND START A NEW GAME",
        "M   TOGGLE SOUND ON OR OFF",
        "F1  OPEN OR CLOSE THIS HELP SCREEN",
        "ESC CLOSE THIS HELP SCREEN"
    ];

    /// <summary>The instruction shown at the bottom of the help overlay.</summary>
    public const string HelpCloseLabel = "PRESS F1 OR ESC TO CLOSE";
}
