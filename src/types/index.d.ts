import type * as Monaco from 'monaco-editor';
export {};

declare global {
  interface Window {
    monaco: typeof Monaco;
    require: any;
  }
}