
async function GetDownloadLinks () {
  // 1) Find and click the Download button
  const download = document.querySelector('div.download .ScenePlayerHeaderPlus-IconItem');
  if (!download) {
    return;
  }
  download.click();

  // 2) Wait a moment for the modal to render
  await new Promise(r => setTimeout(r, 200));

  // 3) Extract all the links
  const options = Array.from(document.querySelectorAll('.VideoJSPlayer-DownloadOption-Link'));
  const data = options.map(a => {
    const parts = Array.from(a.querySelectorAll('span'))
                       .map(s => s.innerText.trim());
    return {
      href:   a.href,
      label:  parts.join(' · ')
    };
  });

  // 4) Hide the modal again
  const closeDownload = document.querySelector('.Modal-CloseButton-Icon-Svg');
  closeDownload.click();

  // 5) Return the array
  return data;
};

(async function() {
  const titleEl = document.querySelector('.Title');
  const title = titleEl
    ? titleEl.innerText.trim()
    : '';

  const descrEl= document.querySelector('.Description-Paragraph');
  const descr = descrEl
    ? descrEl.innerText.trim()
    : '';

  // 1. Select all the actor-name links
  const nameLinks = document.querySelectorAll(
    '.component-ActorThumb-List a.Link.ActorThumb-Name-Link'
  );

  // 2. Extract the ID from each href
  const actorIds = Array.from(nameLinks)
    .map(a => {
      // href is something like "/en/pornstar/view/Alexandra/38441"
      const href = a.getAttribute('href');
      const m = href.match(/\/view\/[^\/]+\/(\d+)(?:\/|$)/);
      return m ? parseInt(m[1], 10) : null;
    })
    .filter(id => id !== null);

  const sceneDateEl = document.querySelector('.ScenePlayerHeaderPlus-SceneDate-Text');
  const sceneDate = sceneDateEl
    ? sceneDateEl.innerText.trim()
    : '';

  const categEl = document.querySelectorAll('.SceneDetails-Category-Link')
  categories = Array.from(categEl).map(el => (el.textContent || '').trim())

  const uri = window.location.href
  const uriMatch = uri.match(/\/video\/([^\/]+)\//)

  const links = await GetDownloadLinks();


  return {
    title,
    description: descr,
    sceneDate,
    actorIds,
    uri: window.location.href,
    categories,
    channel: uriMatch[1],
    links
  };
})()



























async function DetermineFileDetails() {
    const dl = document.querySelector('a.hotosetGalleryInfo-Download-Link');
    if (!dl) return { error: 'not found' };

    // trigger CEF download (this will hit your interceptor)
    dl.click(); 

    // now CEF will fire OnBeforeDownload with the real URL & filename
    return { triggered: true };
}

async function GetGalleries() {
  // 1) find and click the “Pictures” link
  const picAnchor = Array.from(document.querySelectorAll('a[href*="/en/picture/"]'))
    .find(a => a.querySelector('span.Text')?.textContent.trim() === 'Pictures');
  if (!picAnchor) {
    return { error: 'Pictures link not found' };
  }
  picAnchor.click();

  // 2) wait for the pictures page to render
  await new Promise(r => setTimeout(r, 1500));

  // 3) check for the “no screenshots” message
  const contentDiv = document.querySelector('div.content');
  const hasScreens = contentDiv
    ? contentDiv.textContent.includes('There are no screenshots for this scene.')
    : false;

  if(hasScreens) {
    await DetermineFileDetails();
    /// HERE IS WHERE THE DOWNLOAD LINK AND FILE NAME WILL BE PICKED UP BY THE DOWNLOADINTERCEPTOR
  }

  // 4) navigate back
  history.back();
  await new Promise(r => setTimeout(r, 1000));

  
}








































// 1. Grab every actor ID from the name-links:
function getActorIds() {
  return Array.from(
    document.querySelectorAll('.component-ActorThumb-List a.Link.ActorThumb-Name-Link')
  )
  .map(a => {
    const m = a.getAttribute('href').match(/\/view\/[^\/]+\/(\d+)(?:\/|$)/);
    return m ? parseInt(m[1], 10) : null;
  })
  .filter(Boolean);
}

// 2. Grab every image URL and strip width/height, leaving only format=webp:
function getActorImageUrls() {
  return Array.from(
    document.querySelectorAll('.component-ActorThumb-List img')
  )
  .map(img => {
    // parse with the URL API
    try {
      const u = new URL(img.src, window.location.origin);
      u.searchParams.delete('width');
      u.searchParams.delete('height');
      return u.href;
    } catch {
      // fallback regex strip if URL parsing fails
      return img.src
        .replace(/([?&])(width=\d+&?)/g, '$1')
        .replace(/([?&])(height=\d+&?)/g, '$1')
        .replace('?&', '?')
        .replace(/\?$/, '');
    }
  })
  .filter(Boolean);
}

// Usage:
const ids    = getActorIds();
const images = getActorImageUrls();

console.log(ids);    // [38441, …]
console.log(images); // ["https://transform.gammacdn.com/actors/38441/38441_500x750.jpg?format=webp", …]



