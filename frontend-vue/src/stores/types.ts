import { Store } from "pinia";

export declare type LanguageStore = Store<any, any, any, any> &
  Store<
    string,
    {
      language: string;
    },
    {},
    {
      setLanguage: (
        // eslint-disable-next-line no-unused-vars
        language: string
      ) => void;
    }
  >;

export declare type LoadingStateStore = Store<any, any, any, any> &
  Store<
    string,
    {
      loading: boolean;
    },
    {},
    {
      setLoading: (
        // eslint-disable-next-line no-unused-vars
        state: boolean
      ) => void;
    }
  >;
