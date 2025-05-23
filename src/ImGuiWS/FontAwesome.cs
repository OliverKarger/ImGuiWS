namespace ImGuiWS;

public static class FontAwesome {
    public enum FaIcon {
        Home,
        User,
        Cog,
        Envelope,
        Heart,
        Check,
        Times,
        Search,
        Bars,
        Trash,
        File,
        Star,
        Comment,
        Clock,
        Smile,
        Image,
        ThumbsUp
    }

    public enum FaIconStyle {
        Solid,
        Regular
    }

    private static readonly Dictionary<(FaIcon, FaIconStyle), String> IconMap = new() {
        // Solid icons
        { (FaIcon.Home, FaIconStyle.Solid), "\uf015" },
        { (FaIcon.User, FaIconStyle.Solid), "\uf007" },
        { (FaIcon.Cog, FaIconStyle.Solid), "\uf013" },
        { (FaIcon.Envelope, FaIconStyle.Solid), "\uf0e0" },
        { (FaIcon.Heart, FaIconStyle.Solid), "\uf004" },
        { (FaIcon.Check, FaIconStyle.Solid), "\uf00c" },
        { (FaIcon.Times, FaIconStyle.Solid), "\uf00d" },
        { (FaIcon.Search, FaIconStyle.Solid), "\uf002" },
        { (FaIcon.Bars, FaIconStyle.Solid), "\uf0c9" },
        { (FaIcon.Trash, FaIconStyle.Solid), "\uf1f8" },
        { (FaIcon.File, FaIconStyle.Solid), "\uf15b" },
        { (FaIcon.Star, FaIconStyle.Solid), "\uf005" },
        { (FaIcon.Comment, FaIconStyle.Solid), "\uf075" },
        { (FaIcon.Clock, FaIconStyle.Solid), "\uf017" },
        { (FaIcon.Smile, FaIconStyle.Solid), "\uf118" },
        { (FaIcon.Image, FaIconStyle.Solid), "\uf03e" },
        { (FaIcon.ThumbsUp, FaIconStyle.Solid), "\uf164" },

        // Regular icons
        { (FaIcon.Heart, FaIconStyle.Regular), "\uf004" },
        { (FaIcon.Envelope, FaIconStyle.Regular), "\uf0e0" },
        { (FaIcon.File, FaIconStyle.Regular), "\uf15b" },
        { (FaIcon.Star, FaIconStyle.Regular), "\uf005" },
        { (FaIcon.Comment, FaIconStyle.Regular), "\uf075" },
        { (FaIcon.Clock, FaIconStyle.Regular), "\uf017" },
        { (FaIcon.Smile, FaIconStyle.Regular), "\uf118" },
        { (FaIcon.Image, FaIconStyle.Regular), "\uf03e" },
        { (FaIcon.ThumbsUp, FaIconStyle.Regular), "\uf164" }
    };

    public static String Get(FaIcon icon, FaIconStyle style = FaIconStyle.Solid) {
        if(IconMap.TryGetValue((icon, style), out String? unicode)) {
            return unicode;
        }

        throw new ArgumentException($"Icon '{icon}' with style '{style}' is not defined.");
    }
}