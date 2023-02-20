import u from '@/utils';
import ReactMarkdown from 'react-markdown';

export default function IndexPage(props: any) {
  const { md } = props;

  if (u.isEmpty(md)) {
    return null;
  }
  return (
    <>
      <ReactMarkdown remarkPlugins={[]}>{md}</ReactMarkdown>
    </>
  );
}
