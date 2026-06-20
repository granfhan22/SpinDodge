// Shared screen-transition effect: a giant spinning top rolls across the
// screen, then either navigates to targetUrl or runs onDone (used by the
// "Quit" button, which has nowhere to navigate to).
const SPIN_TRANSITION_DURATION_MS = 1100;

function spinTransition(targetUrl, onDone) {
  const overlay = document.createElement('div');
  overlay.className = 'spin-transition-overlay';

  const top = document.createElement('div');
  top.className = 'spin-transition-top';
  top.style.animationDuration = SPIN_TRANSITION_DURATION_MS + 'ms';
  overlay.appendChild(top);
  document.body.appendChild(overlay);

  let done = false;
  function finish() {
    if (done) return;
    done = true;
    if (targetUrl) {
      window.location.href = targetUrl;
    } else {
      overlay.remove();
      if (onDone) onDone();
    }
  }

  top.addEventListener('animationend', finish);
  // Fallback in case animationend doesn't fire (e.g. the tab was backgrounded
  // mid-animation), so screens never get stuck without transitioning.
  setTimeout(finish, SPIN_TRANSITION_DURATION_MS + 200);
}
