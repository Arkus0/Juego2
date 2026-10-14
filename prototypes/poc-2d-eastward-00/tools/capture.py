# Playwright evidence: screenshots at two window sizes, light on/off, true-1x canvas grabs, recorded walk (webm)
import time, os, sys, shutil
from playwright.sync_api import sync_playwright
from PIL import Image
URL=sys.argv[1] if len(sys.argv)>1 else 'http://localhost:4173/'
E=os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))),'evidence'); os.makedirs(E,exist_ok=True)
ARGS=['--use-angle=swiftshader','--enable-unsafe-swiftshader']
def wait(pg):
    pg.wait_for_function('window.__poc && window.__poc.ready',timeout=20000); time.sleep(1.2)
with sync_playwright() as p:
    b=p.chromium.launch(args=ARGS); errs=[]
    for (w,h) in ((1280,720),(1920,1080)):
        for light in ('1','0'):
            pg=b.new_page(viewport={'width':w,'height':h}); pg.on('pageerror',lambda e: errs.append(str(e)))
            pg.on('console',lambda m: errs.append(m.text) if m.type=='error' else None)
            pg.goto(URL+f'?light={light}'); wait(pg)
            tag=f'{w}x{h}_luz{"ON" if light=="1" else "OFF"}'
            pg.screenshot(path=f'{E}/shot_{tag}_spawn.png')
            # move to the chigre front
            pg.keyboard.down('ArrowUp'); time.sleep(1.1); pg.keyboard.up('ArrowUp'); time.sleep(0.6)
            pg.screenshot(path=f'{E}/shot_{tag}_chigre.png')
            if w==1280:
                z=pg.evaluate('__game.scale.zoom'); el=pg.query_selector('canvas'); el.screenshot(path='/tmp/c.png')
                im=Image.open('/tmp/c.png'); im.resize((im.width//z,im.height//z),Image.NEAREST).save(f'{E}/frame_1x_{"luzON" if light=="1" else "luzOFF"}.png')
                print('zoom',z,'canvas',im.size)
            pg.close()
    # recorded walk
    ctx=b.new_context(viewport={'width':1280,'height':720},record_video_dir='/tmp/vid',record_video_size={'width':1280,'height':720})
    pg=ctx.new_page(); pg.goto(URL+'?auto=1'); wait(pg)
    t0=time.time(); frames=0
    while time.time()-t0<30:
        if frames%10==0: pg.screenshot(path=f'/tmp/seq_{frames//10:03d}.png')
        frames+=1; time.sleep(0.1)
        if pg.evaluate('__poc.scene.auto.length')==0: break
    print('walk seconds',round(time.time()-t0,1))
    vp=pg.video.path(); ctx.close(); shutil.copy(vp,f'{E}/recorrido_auto.webm')
    print('errors',errs)
    b.close()
