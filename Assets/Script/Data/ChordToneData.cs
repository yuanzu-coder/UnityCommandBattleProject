[System.Serializable]

public static class ChordToneData
{
    public static int[] GetIntervals(ChordType type)
    {
        switch (type)
        {
            case ChordType.Major: return new int[] {0, 4, 7};
            case ChordType.Minor: return new int[] {0, 3, 7};
            case ChordType.Diminish: return new int[] {0, 3, 6};
            case ChordType.Augment: return new int[] {0, 4, 8};
            case ChordType.Dominant7: return new int[] {0, 4, 7, 10};
            case ChordType.Major7: return new int[] {0, 4, 7, 11};
            case ChordType.Minor7: return new int[] {0, 3, 7, 10};
            case ChordType.Minor7b5: return new int[] {0, 3, 6, 10};
        }

        return new int[] {0, 4, 7};
    }
}
