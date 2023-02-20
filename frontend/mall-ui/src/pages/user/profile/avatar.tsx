import u from '@/utils';
import {
    Avatar
} from '@mui/material';
import XItem from './item';

export default function Animations(props: any) {
  const { model, ok } = props;

  return (
    <>
      <XItem
        title={'头像'}
        right={
          <Avatar
            variant="square"
            sx={{
              width: 80,
              height: 80,
              borderRadius: 1,
            }}
            src={u.resolveAvatar(model.Avatar, {
              width: 80,
              height: 80,
            })}
          >
            {model.NickName || model.IdentityName || '--'}
          </Avatar>
        }
      />
    </>
  );
}
