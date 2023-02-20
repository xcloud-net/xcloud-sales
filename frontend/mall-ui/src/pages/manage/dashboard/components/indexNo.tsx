import u from '@/utils';
import { Tag } from 'antd';

const colors = [
  {
    index: 0,
    color: 'success',
  },
  {
    index: 1,
    color: 'error',
  },
  {
    index: 2,
    color: 'warning',
  },
];

interface IIndexNoProps {
  index: number;
}

export default (props: IIndexNoProps) => {
  const { index } = props;

  var color = u.find(colors, (x) => x.index == index)?.color;
  color = color || 'default';

  return <Tag color={color}>#{index + 1}</Tag>;
};
