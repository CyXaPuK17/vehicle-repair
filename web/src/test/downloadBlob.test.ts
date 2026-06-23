import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { downloadBlob } from '../components/common/downloadBlob';

describe('downloadBlob', () => {
  const FAKE_URL = 'blob:fake-url';
  let clickCount = 0;
  let capturedAnchor: HTMLAnchorElement | null = null;

  beforeEach(() => {
    clickCount = 0;
    capturedAnchor = null;

    vi.spyOn(URL, 'createObjectURL').mockReturnValue(FAKE_URL);
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => {});

    const realCreate = HTMLDocument.prototype.createElement.bind(document);
    vi.spyOn(document, 'createElement').mockImplementation((tag: string) => {
      const el = realCreate(tag);
      if (tag === 'a') {
        capturedAnchor = el as HTMLAnchorElement;
        Object.defineProperty(el, 'click', {
          value: () => { clickCount++; },
          configurable: true,
        });
      }
      return el;
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('creates an object URL from the blob', () => {
    const blob = new Blob(['test'], { type: 'text/plain' });
    downloadBlob(blob, 'test.txt');
    expect(URL.createObjectURL).toHaveBeenCalledWith(blob);
  });

  it('sets href and download attribute on the anchor', () => {
    const blob = new Blob(['data']);
    downloadBlob(blob, 'report.xlsx');
    expect(capturedAnchor).not.toBeNull();
    expect(capturedAnchor!.href).toContain(FAKE_URL);
    expect(capturedAnchor!.download).toBe('report.xlsx');
  });

  it('triggers a click on the anchor element', () => {
    downloadBlob(new Blob(['data']), 'file.pdf');
    expect(clickCount).toBe(1);
  });

  it('revokes the object URL after clicking', () => {
    downloadBlob(new Blob(['data']), 'file.pdf');
    expect(URL.revokeObjectURL).toHaveBeenCalledWith(FAKE_URL);
  });
});
