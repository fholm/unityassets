using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;

public class BattleTextFont
{
    public enum DefinitionType
    {
        BMFont_XML
    }

    static Dictionary<string, BattleTextFont> fonts = new Dictionary<string, BattleTextFont>();

    public string Name { get; private set; }
    public float GlyphSize { get; private set; }
    public float TextureSize { get; private set; }
    public BattleTextGlyph[] Glyphs { get; private set; }
    public Material Material { get; private set; }

    BattleTextFont()
    {
        Glyphs = new BattleTextGlyph[256];
    }

    public BattleTextSentance MakeSentance(string sentance)
    {
        return new BattleTextSentance(this, sentance);
    }

    public BattleTextGlyph[] GetGlyphs(string sentance)
    {
        sentance = sentance ?? "";

        BattleTextGlyph[] glyphs = new BattleTextGlyph[sentance.Length];

        for (int i = 0; i < sentance.Length; ++i)
        {
            int c = (int)sentance[i];

            if (c >= 0 && c <= 255)
            {
                glyphs[i] = Glyphs[c];
            }
            else
            {
                Debug.Log("No support for " + sentance[i]);
            }
        }

        return glyphs;
    }

    public static BattleTextFont Load(TextAsset asset)
    {
        if (asset == null)
        {
            Debug.LogError("[NamePlates] Text asset was null");
            return null;
        }

        BattleTextFont font;

        if (!fonts.TryGetValue(asset.name, out font))
        {
            font = new BattleTextFont();
            StringReader reader = new StringReader(asset.text);
            XmlReader xml = XmlReader.Create(reader);

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    switch (xml.Name)
                    {
                        case "info":
                            font.Name = xml.GetAttribute(0);
                            font.GlyphSize = Int32.Parse(xml.GetAttribute(1));
                            break;

                        case "common":
                            font.TextureSize = Single.Parse(xml.GetAttribute(2));
                            break;

                        case "char":
                            int id = Int32.Parse(xml.GetAttribute(0));
                            float x = Single.Parse(xml.GetAttribute(1));
                            float y = Single.Parse(xml.GetAttribute(2));
                            float width = Single.Parse(xml.GetAttribute(3));
                            float height = Single.Parse(xml.GetAttribute(4));
                            float offset = Single.Parse(xml.GetAttribute(6));

                            font.Glyphs[id] = new BattleTextGlyph(id, x, y, width, height, offset);

                            break;
                    }
                }
            }

            fonts.Add(asset.name, font);
        }

        return font;
    }

    public override string ToString()
    {
        return String.Format("<NPFont:{0}:fontSize={1}:textureSize={2}>", Name, GlyphSize, TextureSize);
    }
}

public class BattleTextGlyph
{
    public readonly int Id;
    public readonly float X;
    public readonly float Y;
    public readonly float Width;
    public readonly float Height;
    public readonly float Offset;

    public BattleTextGlyph(int id, float x, float y, float width, float height, float offset)
    {
        Id = id;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Offset = offset;
    }

    public float CalculateWidth(BattleTextFont font)
    {
        return Width / font.GlyphSize;
    }

    public float CalculateHeight(BattleTextFont font)
    {
        return Height / font.GlyphSize;
    }

    public float CalculateOffset(BattleTextFont font)
    {
        return Offset / font.GlyphSize;
    }

    public override string ToString()
    {
        return String.Format("<NPGlyph:{0}:x={1}:y={2}:width={3}:height={4}>", Id, X, Y, Width, Height);
    }
}

public class BattleTextSentance
{
    readonly BattleTextGlyph[] glyphs;

    public readonly BattleTextFont Font;
    public readonly string Sentance;
    public readonly int GlyphCount;

    public float CalculateWidth(float space)
    {
        float width = 0f;

        for (int i = 0; i < glyphs.Length; ++i)
        {
            if (i > 0)
            {
                width += space;
            }

            width += glyphs[i].CalculateWidth(Font);
        }

        return width;
    }

    public BattleTextSentance(BattleTextFont font, string sentance)
    {
        Font = font;
        Sentance = sentance;
        glyphs = font.GetGlyphs(sentance);
        GlyphCount = glyphs.Length;
    }

    public BattleTextGlyph this[int index]
    {
        get
        {
            return glyphs[index];
        }
    }
}