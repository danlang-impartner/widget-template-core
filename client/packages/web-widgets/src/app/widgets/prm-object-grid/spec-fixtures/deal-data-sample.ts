import { prm, prmObjects } from '@impartner/api';

export const testDataOfPrmObject: prm.IThqlData<prmObjects.Deal> = {
  count: 10,
  entity: 'Deal',
  results: [
    {
      id: 1,
      name: 'QATest Deal6',
      company: {
        id: 1,
        name: ''
      },
      source: 'Online',
      amount: 1000,
      probability: 0.15,
      stage: {
        name: 'Ordered'
      },
      closeDate: '2017-06-15T00:00:00'
    },
    {
      id: 2,
      name: 'QATest Deal2',
      company: {
        id: 1,
        name: 'Company1 Co'
      },
      source: 'Online',
      amount: 1000,
      probability: 1,
      stage: {
        name: 'Ordered'
      },
      closeDate: '2017-06-29T00:00:00'
    },
    {
      id: 3,
      name: 'Old Close date?',
      company: {
        id: 1,
        name: ''
      },
      source: 'Referral',
      amount: 1000,
      probability: 0.1,
      stage: {
        name: 'New Lead'
      },
      closeDate: '2016-11-17T00:00:00'
    }
  ]
};
