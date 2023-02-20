import { Button, Card, message, Select, Table } from 'antd';
import { useEffect, useState } from 'react';
import XBalance from './balance';

import XUserAvatar from '@/components/manage/user/avatar';
import u from '@/utils';
import http from '@/utils/http';
import { MallUserDto } from '@/utils/models';
import { ColumnProps } from 'antd/es/table';
import { history } from 'umi';
import XStatus from './updateStatus';
import XSearchForm from './searchForm';
import XTime from '@/components/manage/time';

export default (props: any) => {
  const [query, _query] = useState({
    Page: 1,
    PageSize: 20,
    TotalCount: 1,
  });
  const [loading, _loading] = useState(false);
  const [loadingId, _loadingId] = useState(0);
  const [loadingSetGradeId, _loadingSetGradeId] = useState(0);
  const [datalist, _datalist] = useState<any[]>([]);
  const [gradelist, _gradelist] = useState<any[]>([]);

  const queryGradeList = () => {
    http.apiRequest
      .post('/mall-admin/user-grade/list', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _gradelist(res.data.Data || []);
        }
      })
      .finally(() => {});
  };

  const queryList = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/user/paging', {
        ...query,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _query({
            ...query,
            TotalCount: res.data.TotalCount,
          });
          _datalist(res.data.Items || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const setUserGrade = (row: any, gradeId: any) => {
    if (!confirm('确定设置？')) {
      return;
    }
    _loadingSetGradeId(row.Id);
    http.apiRequest
      .post('/mall-admin/user-grade/set-user-grade', {
        UserId: row.Id,
        GradeId: gradeId == -1 ? '' : gradeId,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('设置成功');
          queryList();
        }
      })
      .finally(() => {
        _loadingSetGradeId(0);
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '账号主体',
      render: (x: MallUserDto) => <XUserAvatar model={x.SysUser} />,
    },
    {
      title: '会员名',
      render: (x: MallUserDto) => u.simpleString(x.NickName || '--', 5),
    },
    {
      title: '会员等级',
      render: (x) => {
        return (
          <Select
            size="small"
            loading={loadingSetGradeId == x.Id}
            onChange={(e) => {
              setUserGrade(x, e);
            }}
            value={u.isEmpty(x.GradeId) ? -1 : x.GradeId}
            placeholder="选择会员等级"
            style={{
              minWidth: 150,
            }}
          >
            <Select.Option key={-1} value={-1}>
              无等级
            </Select.Option>
            {u.map(gradelist, (d) => (
              <Select.Option key={d.Id} value={d.Id}>
                {d.Name}
              </Select.Option>
            ))}
          </Select>
        );
      },
    },
    {
      title: '手机号',
      render: (x: MallUserDto) => x.SysUser?.AccountMobile || '--',
    },
    {
      title: '账户余额',
      render: (x) => (
        <XBalance
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '余额积分',
      render: (x) => x.Points,
    },
    {
      title: '历史积分',
      render: (x) => x.HistoryPoints,
    },
    {
      title: '状态',
      render: (x) => (
        <XStatus
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '最近上线时间',
      render: (x: MallUserDto) =>
        u.isEmpty(x.LastActivityTime) ||
        u.dayjs(x.LastActivityTime).add(u.timezoneOffset, 'hour').fromNow(),
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: 200,
      render: (x) => {
        return (
          <Button.Group size="small">
            <Button
              onClick={() => {
                alert('todo');
              }}
            >
              查看
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, [query.Page]);

  useEffect(() => {
    queryGradeList();
  }, []);

  return (
    <>
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        title="商城会员"
        size="small"
        loading={loading}
        style={{
          marginBottom: 10,
        }}
        extra={
          <Button
            size="small"
            type="text"
            onClick={() => {
              history.push('/manage/settings/user/list');
            }}
          >
            查看平台用户信息
          </Button>
        }
      >
        <Table
          size="small"
          dataSource={datalist}
          columns={columns}
          pagination={{
            pageSize: query.PageSize,
            total: query.TotalCount,
            onChange: (page, size) => {
              _query({
                ...query,
                Page: page,
              });
            },
          }}
        />
      </Card>
    </>
  );
};
