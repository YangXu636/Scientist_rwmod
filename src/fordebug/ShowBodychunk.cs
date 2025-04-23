using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;

namespace Scientist.Debug;

public class ShowBodychunk
{
    public FSprite[] bodychunkSprites;
    public FLabel[] indexLabel;
    public FLabel posLabel;
    public LinkLine line;

    public IntVector2 posInt = new IntVector2(0, 0);
    public IntVector2 sizeInt = new IntVector2(0, 0);

    public ShowBodychunk(BodyChunk[] bodychunks, Vector2 rCamPos)
    {
        this.bodychunkSprites = new FSprite[bodychunks.Length];
        this.indexLabel = new FLabel[bodychunks.Length];
        for (int i = 0; i < bodychunks.Length; i++)
        {
            this.bodychunkSprites[i] = new FSprite("Circle20", true) { alpha = 0.7f };
            this.bodychunkSprites[i].scale = bodychunks[i].rad / 10.00f;
            this.indexLabel[i] = new(Custom.GetFont(), $"{i}") { color = ScientistTools.ColorFromHex("#000004") };
        }
        this.posLabel = new FLabel(Custom.GetFont(), "") { color = ScientistTools.ColorFromHex("#CCE5FF") };
        this.sizeInt = new IntVector2(Mathf.CeilToInt(this.posLabel.textRect.size.x / 20.00f), Mathf.CeilToInt(this.posLabel.textRect.size.y / 20.00f));
    }

    public void AddSpritesToContainer(FContainer container)
    {
        for (int i = 0; i < this.bodychunkSprites.Length; i++)
        {
            container.AddChild(this.bodychunkSprites[i]);
            container.AddChild(this.indexLabel[i]);
        }
        container.AddChild(this.posLabel);
        //line.AddSpritesToContainer(container);
    }

    public void Update(BodyChunk[] bodychunks, Vector2 rCamPos)
    {
        this.posLabel.text = "";
        int xMin = int.MaxValue, xMax = int.MinValue, yMin = int.MaxValue, yMax = int.MinValue;
        for (int i = 0; i < bodychunks.Length; i++)
        {
            this.bodychunkSprites[i].x = bodychunks[i].pos.x - rCamPos.x;
            this.bodychunkSprites[i].y = bodychunks[i].pos.y - rCamPos.y;
            int xxMin = Mathf.RoundToInt((this.bodychunkSprites[i].x - bodychunks[i].rad) / 20.00f);
            int xxMax = Mathf.RoundToInt((this.bodychunkSprites[i].x + bodychunks[i].rad) / 20.00f);
            int yyMin = Mathf.RoundToInt((this.bodychunkSprites[i].y - bodychunks[i].rad) / 20.00f);
            int yyMax = Mathf.RoundToInt((this.bodychunkSprites[i].y + bodychunks[i].rad) / 20.00f);
            Scientist.Data.DebugVariables.ShowbodychunksGrid.SetAll(true, xxMin, xxMax, yyMin, yyMax);
            xMin = Math.Min(xMin, xxMin); xMax = Math.Max(xMax, xxMax); yMin = Math.Min(yMin, yyMin); yMax = Math.Max(yMax, yyMax);
            this.bodychunkSprites[i].scale = bodychunks[i].rad / 10.00f;
            this.indexLabel[i].x = bodychunks[i].pos.x - rCamPos.x;
            this.indexLabel[i].y = bodychunks[i].pos.y - rCamPos.y;
            this.posLabel.text += $"{(i == 0 ? "" : "\n")}{bodychunks[i].pos}";
        }
        this.sizeInt = new IntVector2(Mathf.RoundToInt(this.posLabel.textRect.size.x / 20.00f), Mathf.RoundToInt(this.posLabel.textRect.size.y / 20.00f));
        //ScientistLogger.Log($"sizeInt = {sizeInt}");
        //×ó
        this.posLabel.x = Mathf.Clamp(xMin - this.sizeInt.x + 2, 0, Scientist.Data.DebugVariables.ShowbodychunksGrid.GetLength(0) - this.sizeInt.x) * 20f;
        this.posLabel.y = Mathf.Clamp(yMin + 1, 0, Scientist.Data.DebugVariables.ShowbodychunksGrid.GetLength(1) - this.sizeInt.y) * 20f;
    }

    public void RemoveSprites()
    {
        for (int i = 0; i < this.bodychunkSprites.Length; i++)
        {
            this.bodychunkSprites[i].RemoveFromContainer();
            this.bodychunkSprites[i] = null;
        }
        for (int i = 0; i < this.indexLabel.Length; i++)
        {
            this.indexLabel[i].RemoveFromContainer();
            this.indexLabel[i] = null;
        }
        this.posLabel.RemoveFromContainer();
        this.posLabel = null;
        //this.line.RemoveSprites();
        //this.line = null;
    }

    public class LinkLine
    {
        public FSprite line;
        public Vector2 startPos;
        public Vector2 endPos;
        public float width;
        public Color color;

        public LinkLine(Vector2 startPos, Vector2 endPos, float width, Color color)
        {
            this.line = new FSprite("pixel", true) { alpha = 0.7f, color = color };
            this.startPos = startPos;
            this.endPos = endPos;
            this.width = width;
            this.color = color;
        }

        public void Update(Vector2 startPos, Vector2 endPos)
        {
            this.line.scaleY = width;
            this.line.scaleX = Vector2.Distance(startPos, endPos);
            this.line.x = (startPos.x + endPos.x) / 2.00f;
            this.line.y = (startPos.y + endPos.y) / 2.00f;
            this.line.rotation = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        }

        public void AddSpritesToContainer(FContainer container)
        {
            container.AddChild(this.line);
        }

        public void RemoveSprites()
        {
            this.line.RemoveFromContainer();
            this.line = null;
        }
    }
}