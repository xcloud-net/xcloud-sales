import XDetailDialog from '@/components/detailDialog';
import XDetail from './index';
import u from '@/utils';

const index = function(props: { detailId: string, onClose: any }) {
  const { detailId, onClose } = props;

  const active = !u.isEmpty(detailId);

  return (
    <>
      <XDetailDialog open={active} onClose={onClose}>
        <XDetail detailId={detailId} />
      </XDetailDialog>
    </>
  );
};

export default index;
