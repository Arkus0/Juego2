import Phaser from 'phaser';

const LW = 480, LH = 270;               // logical resolution (integer-scaled)
const params = new URLSearchParams(location.search);

function integerZoom() {
  return Math.max(1, Math.floor(Math.min(window.innerWidth / LW, window.innerHeight / LH)));
}

type TObj = { name: string; type: string; x: number; y: number; width: number; height: number; properties?: { name: string; value: unknown }[] };
const prop = (o: TObj, k: string) => o.properties?.find(p => p.name === k)?.value;

class Street extends Phaser.Scene {
  player!: Phaser.Physics.Arcade.Sprite;
  shadow!: Phaser.GameObjects.Ellipse;
  keys!: Record<string, Phaser.Input.Keyboard.Key>;
  lightObjs: Phaser.GameObjects.GameObject[] = [];
  solids!: Phaser.Physics.Arcade.StaticGroup;
  auto: { x: number; y: number }[] = [];
  constructor() { super('street'); }

  preload() {
    this.load.tilemapTiledJSON('street', 'maps/street_00.json');
    this.load.image('pavement', 'tilesets/pavement_16.png');
    this.load.atlas('world', 'atlas/world_atlas.png', 'atlas/world_atlas.json');
  }

  create() {
    const map = this.make.tilemap({ key: 'street' });
    const ts = map.addTilesetImage('pavement_16', 'pavement')!;
    map.createLayer('ground', ts, 0, 0)!.setDepth(-100);
    const foot = 200;
    // backdrop behind roofs (evening sky band) — flat colour, no art pretending to be art
    this.add.rectangle(0, 0, map.widthInPixels, foot - 20, 0x2a2430).setOrigin(0).setDepth(-200);

    const objs = (map.getObjectLayer('objects')!.objects as unknown) as TObj[];
    for (const o of objs) {
      if (o.type === 'facade') {
        const f = String(prop(o, 'frame')); const flip = Boolean(prop(o, 'flipX'));
        // modular stack: base + upper (world depth = foot line) and roof (occlusion layer above actors)
        const parts = ['base', 'upper', 'roof'].map(p => this.textures.getFrame('world', `${f}_${p}`));
        let y = o.y;
        for (const [i, fr] of parts.entries()) {
          const img = this.add.image(o.x + o.width / 2, y, 'world', fr.name).setOrigin(0.5, 1).setFlipX(flip);
          img.setDepth(i === 2 ? 5000 : o.y);
          y -= fr.height;
        }
      } else if (o.type === 'prop' || o.type === 'prop_wall') {
        const img = this.add.image(o.x, o.y, 'world', String(prop(o, 'frame'))).setOrigin(0.5, 1);
        img.setDepth(o.type === 'prop_wall' ? foot + 1 : o.y);
      } else if (o.type === 'light') {
        const r = Number(prop(o, 'radius'));
        this.lightObjs.push(this.add.image(o.x, o.y, 'glow').setScale(r / 32).setBlendMode(Phaser.BlendModes.ADD).setAlpha(0.55).setDepth(8001));
      }
    }
    // glow texture: banded radial (pixel-art friendly)
    this.solids = this.physics.add.staticGroup();
    for (const c of (map.getObjectLayer('collision')!.objects as unknown) as TObj[]) {
      const z = this.add.zone(c.x + c.width / 2, c.y + c.height / 2, c.width, c.height);
      this.physics.add.existing(z, true); this.solids.add(z);
    }
    // PROVISIONAL player capsule (14x30), clearly not final art
    const spawn = objs.find(o => o.name === 'player_spawn')!;
    this.shadow = this.add.ellipse(spawn.x, spawn.y, 14, 5, 0x120f14, 0.45);
    this.player = this.physics.add.sprite(spawn.x, spawn.y, 'capsule').setOrigin(0.5, 1);
    (this.player.body as Phaser.Physics.Arcade.Body).setSize(10, 5).setOffset(2, 25);
    this.player.setCollideWorldBounds(true);
    this.physics.world.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
    this.physics.add.collider(this.player, this.solids);
    const cam = this.cameras.main;
    cam.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
    cam.startFollow(this.player, true, 0.12, 0.12);
    cam.setRoundPixels(true);
    // lighting overlay (subordinate to the art; L toggles)
    // dusk grade: translucent warm-dark wash (works in Canvas and WebGL; MULTIPLY is not reliable in Canvas)
    const tint = this.add.rectangle(0, 0, LW, LH, 0x2a1820, 0.32).setOrigin(0).setScrollFactor(0).setDepth(8000);
    this.lightObjs.push(tint);
    const on = params.get('light') !== '0';
    this.lightObjs.forEach(o => (o as Phaser.GameObjects.Image).setVisible(on));
    this.keys = this.input.keyboard!.addKeys('W,A,S,D,UP,DOWN,LEFT,RIGHT,L,C') as Record<string, Phaser.Input.Keyboard.Key>;
    this.keys.L.on('down', () => this.lightObjs.forEach(o => { const i = o as Phaser.GameObjects.Image; i.setVisible(!i.visible); }));
    this.keys.C.on('down', () => { this.physics.world.drawDebug = !this.physics.world.drawDebug; if (!this.physics.world.debugGraphic) this.physics.world.createDebugGraphic(); this.physics.world.debugGraphic.clear(); });
    if (params.get('auto') === '1') this.auto = [[226, 300], [226, 236], [120, 236], [60, 290], [60, 380], [400, 380], [430, 240], [300, 232], [232, 216], [226, 300]].map(([x, y]) => ({ x, y }));
    (window as any).__poc = { scene: this, ready: true };
  }

  update() {
    const k = this.keys; let vx = 0, vy = 0;
    if (this.auto.length) {
      const t = this.auto[0]; const dx = t.x - this.player.x, dy = t.y - this.player.y;
      if (Math.hypot(dx, dy) < 3) this.auto.shift(); else { vx = Math.sign(Math.round(dx / 3)); vy = Math.sign(Math.round(dy / 3)); }
    } else {
      if (k.LEFT.isDown || k.A.isDown) vx = -1; else if (k.RIGHT.isDown || k.D.isDown) vx = 1;
      if (k.UP.isDown || k.W.isDown) vy = -1; else if (k.DOWN.isDown || k.S.isDown) vy = 1;
    }
    const sp = 64, n = vx && vy ? Math.SQRT1_2 : 1;
    this.player.setVelocity(vx * sp * n, vy * sp * n);
    this.player.setDepth(this.player.y);
    this.shadow.setPosition(this.player.x, this.player.y - 1).setDepth(this.player.y - 0.5);
  }
}

class Boot extends Phaser.Scene {
  constructor() { super('boot'); }
  create() {
    const g = this.make.graphics({}, false);
    g.fillStyle(0x1a1418).fillRoundedRect(0, 0, 14, 30, 6);
    g.fillStyle(0x9a9aa8).fillRoundedRect(1, 1, 12, 28, 5);
    g.fillStyle(0xd0d0dc).fillRect(3, 3, 3, 10);
    g.fillStyle(0xe05a5a).fillRect(4, 14, 6, 2); // 'PROV' stripe
    g.generateTexture('capsule', 14, 30);
    const c = this.textures.createCanvas('glow', 64, 64)!; const ctx = c.getContext();
    for (let y = 0; y < 64; y++) for (let x = 0; x < 64; x++) {
      const d = Math.hypot(x - 31.5, y - 31.5) / 32; if (d >= 1) continue;
      const a = Math.round((1 - d) ** 2 * 5) / 5;
      ctx.fillStyle = `rgba(214,150,82,${a})`; ctx.fillRect(x, y, 1, 1);
    }
    c.refresh();
    this.scene.start('street');
  }
}

const game = new Phaser.Game({
  // CANVAS by default: Phaser 4.2.1 WebGL TilemapLayer rendered wrong tiles in our headless test (see reports/01-ART-GATE.md); ?r=webgl to compare
  type: params.get('r') === 'webgl' ? Phaser.WEBGL : Phaser.CANVAS, parent: 'game', width: LW, height: LH, zoom: integerZoom(),
  pixelArt: true, roundPixels: true, backgroundColor: '#120f14',
  physics: { default: 'arcade', arcade: { debug: false } },
  scale: { mode: Phaser.Scale.NONE },
  scene: [Boot, Street],
});
window.addEventListener('resize', () => game.scale.setZoom(integerZoom()));
(window as any).__game = game;
