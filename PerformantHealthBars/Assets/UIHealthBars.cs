using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBars : MaskableGraphic, ILayoutElement {
  Int32 ILayoutElement.layoutPriority { get { return 0; } }
  Single ILayoutElement.flexibleHeight { get { return -1; } }
  Single ILayoutElement.flexibleWidth { get { return -1; } }
  Single ILayoutElement.minHeight { get { return 0; } }
  Single ILayoutElement.minWidth { get { return 0; } }
  Single ILayoutElement.preferredHeight { get { return Screen.height; } }
  Single ILayoutElement.preferredWidth { get { return Screen.width; } }

  void ILayoutElement.CalculateLayoutInputHorizontal() { }
  void ILayoutElement.CalculateLayoutInputVertical() { }

  Color _friendly;
  Color _enemy;

  UIVertex[][] _quads_bg;
  UIVertex[][] _quads_fg;

  public Int32 Height = 8;
  public Int32 Border = 2;

  public Color Background;

  protected override void Awake() {
    base.Awake();
    InitArrays();
    raycastTarget = false;
  }

  void Update() {
    SetVerticesDirty();
  }

  protected override void OnPopulateMesh(VertexHelper vh) {
    vh.Clear();

    InitArrays();

    // grab canvas rect
    var canvasRect = canvas.GetComponent<RectTransform>().rect;

    var index = 0;
    var camera = Camera.main;

    foreach (var hb in HealthBar.Active) {
      var pos = hb.transform.position + new Vector3(0, hb.HealthBarYOffset, 0);

      DrawHealthBarWorld(vh, camera, canvasRect, index, pos, hb.HealthPercentage, hb.HealthBarWidth, hb.HealthBarColor, hb.HealthBarAlpha);

      ++index;
    }
  }

  void DrawHealthBarWorld(VertexHelper vh, Camera camera, Rect r, Int32 index, Vector3 world, Single fill, Single width, Color friendly, Single alpha) {
    DrawHealthBarScreen(vh, r, index, camera.WorldToViewportPoint(world), fill, width, friendly, alpha);
  }

  void DrawHealthBarScreen(VertexHelper vh, Rect r, Int32 index, Vector2 screen, Single fill, Single width, Color color, Single alpha) {
    Color bg;
    bg = Background;
    bg.a = alpha;

    Color fg;
    fg = color;
    fg.a = alpha;

    const int BOT_LEFT = 0;
    const int BOT_RIGHT = 1;
    const int TOP_RIGHT = 2;
    const int TOP_LEFT = 3;

    screen.x = r.width * (screen.x);
    screen.y = r.height * (screen.y);

    _quads_bg[index][BOT_LEFT].color = bg;
    _quads_bg[index][BOT_RIGHT].color = bg;
    _quads_bg[index][TOP_RIGHT].color = bg;
    _quads_bg[index][TOP_LEFT].color = bg;

    _quads_fg[index][BOT_LEFT].color = fg;
    _quads_fg[index][BOT_RIGHT].color = fg;
    _quads_fg[index][TOP_RIGHT].color = fg;
    _quads_fg[index][TOP_LEFT].color = fg;

    var bot_left = screen + new Vector2(-width / 2, -Height / 2);
    var bot_right = screen + new Vector2(+width / 2, -Height / 2);
    var top_right = screen + new Vector2(+width / 2, +Height / 2);
    var top_left = screen + new Vector2(-width / 2, +Height / 2);

    _quads_bg[index][BOT_LEFT].position = bot_left;
    _quads_bg[index][BOT_RIGHT].position = bot_right;
    _quads_bg[index][TOP_RIGHT].position = top_right;
    _quads_bg[index][TOP_LEFT].position = top_left;

    vh.AddUIVertexQuad(_quads_bg[index]);

    // only add foreground if we have any fill value
    // otherwise we might get 1px errors when healthbar is empty
    if (fill > 0) {
      bot_left += new Vector2(+Border, +Border);
      bot_right += new Vector2(-Border, +Border);
      top_right += new Vector2(-Border, -Border);
      top_left += new Vector2(+Border, -Border);

      bot_right.x = bot_left.x + ((bot_right.x - bot_left.x) * fill);
      top_right.x = top_left.x + ((top_right.x - top_left.x) * fill);

      _quads_fg[index][BOT_LEFT].position = bot_left;
      _quads_fg[index][BOT_RIGHT].position = bot_right;
      _quads_fg[index][TOP_RIGHT].position = top_right;
      _quads_fg[index][TOP_LEFT].position = top_left;

      vh.AddUIVertexQuad(_quads_fg[index]);
    }
  }

  void InitArrays() {
    if (_quads_bg == null || _quads_fg == null || _quads_bg.Length < HealthBar.Active.Count) {
      _quads_bg = new UIVertex[Math.Max(64, HealthBar.Active.Count)][];
      _quads_fg = new UIVertex[_quads_bg.Length][];

      for (Int32 i = 0; i < _quads_bg.Length; ++i) {
        _quads_bg[i] = new UIVertex[4];
        _quads_fg[i] = new UIVertex[4];
      }
    }
  }
}
