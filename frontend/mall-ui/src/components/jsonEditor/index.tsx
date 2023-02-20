import JSONEditor from 'jsoneditor';
import 'jsoneditor/dist/jsoneditor.css';
import { useEffect, useRef } from 'react';

export default ({ json, ok }: { json?: any; ok?: any }) => {
  const container = useRef<HTMLDivElement | null>(null);
  const editor = useRef<JSONEditor | null>(null);

  useEffect(() => {
    if (container.current != null && editor.current == null) {
      editor.current = new JSONEditor(container.current, {
        mode: 'code',
        onChangeText: (e) => {
          ok && ok(e);
        },
      });
    }
  }, [container]);

  useEffect(() => {
    if (editor.current != null) {
      editor.current.set(json);
    }
  }, [json]);

  return (
    <>
      <div ref={container} style={{ width: '100%', height: '100%' }}></div>
    </>
  );
};
