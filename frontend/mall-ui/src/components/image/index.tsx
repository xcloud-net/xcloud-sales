import { Image, ImageProps } from 'antd-mobile';
import empty from '@/assets/empty.svg';

export default (props: ImageProps) => {
  return (
    <Image
      lazy
      width={'100%'}
      placeholder={<img src={empty} width="100%" alt="" />}
      {...props}
    />
  );
};
