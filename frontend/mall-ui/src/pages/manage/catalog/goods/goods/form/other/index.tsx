import XSeo from './seo';
import XTag from './tags';

export default (props: any) => {
  const { data, ok } = props;

  return (
    <>
      <XSeo data={data} ok={ok} />
      <XTag data={data} ok={ok} />
    </>
  );
};
