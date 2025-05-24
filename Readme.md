


(function() {
  const titleEl= document.querySelector('.Title');
  const title= titleEl
    ? titleEl.innerText.trim()
    : '';

  const descrEl= document.querySelector('.Description');
  const descr= descrEl
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

  return {
    title,
    descr,
    actorIds
  };
})()






















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



