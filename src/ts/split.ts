import Split from 'split.js';

export function initSplit(data: SplitOptions) {
  console.log('initSplit', data);
  const options: Split.Options = {
    gutter: (index, direction) => {
      // most code copied from https://github.com/nathancahill/split/tree/master/packages/splitjs
      // but we added a more custom class splitter-gutter
      const gutter = document.createElement('div')
      gutter.className = `gutter gutter-${direction} splitter-gutter`
      return gutter
    },
    ...data.options,
  };
  Split(data.parts, options);
}


interface SplitOptions {
  parts: string[];
  options: Split.Options;
}